using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RfidApi.Data;
using RfidApi.Models;
using RfidApi.Models.Dtos;
using static RfidApi.Errors.CustomErrorHandlers;

namespace RfidApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context) => _context = context;

    // GET: api/items
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery(Name = "page_size")] int pageSize = 20,
        [FromQuery(Name = "owner_email")] string? ownerEmail = null,
        [FromQuery(Name = "status")] string? status = null,
        [FromQuery(Name = "from_date")] DateTime? fromDate = null,
        [FromQuery(Name = "to_date")] DateTime? toDate = null,
        [FromQuery(Name = "sort_by")] string? sortBy = null,
        [FromQuery(Name = "sort_order")] string? sortOrder = "asc",
        [FromQuery(Name = "search")] string? search = null
    )
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var query = _context.item.Include(i => i.OwnerUser).AsQueryable();

        if (!string.IsNullOrEmpty(ownerEmail))
        {
            query = query
                .Include(i => i.OwnerUser)
                .Where(i => i.OwnerUser != null && i.OwnerUser.Email == ownerEmail);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(i => i.status == status);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(i => i.last_updated >= fromDate.Value);
        }
        if (toDate.HasValue)
        {
            query = query.Where(i => i.last_updated <= toDate.Value);
        }

        // Free text search across name and description (case-insensitive)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowered = search.ToLower();
            query = query.Where(i =>
                (i.name != null && i.name.ToLower().Contains(lowered))
                || (i.description != null && i.description.ToLower().Contains(lowered))
            );
        }

        bool descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = (sortBy, descending) switch
        {
            ("name", false) => query.OrderBy(i => i.name),
            ("name", true) => query.OrderByDescending(i => i.name),
            ("status", false) => query.OrderBy(i => i.status),
            ("status", true) => query.OrderByDescending(i => i.status),
            ("last_updated", false) => query.OrderBy(i => i.last_updated),
            ("last_updated", true) => query.OrderByDescending(i => i.last_updated),
            (_, false) => query.OrderBy(i => i.Id),
            (_, true) => query.OrderByDescending(i => i.Id),
        };

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => item.MapItemToDto())
            .ToListAsync();

        var result = new
        {
            page,
            page_size = pageSize,
            total = totalItems,
            items,
        };

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Add([FromBody] CreateItemDto dto)
    {
        var retailer = await _context.Retailers.FirstOrDefaultAsync(r =>
            r.Id == dto.Retailer_id && r.Accredited
        );

        if (retailer == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ForbiddenProblem());
        }

        Item newItem = dto.MapDtoToItem();
        await _context.item.AddAsync(newItem);
        try
        {
            await _context.SaveChangesAsync();

            // Create Item event before return

            ItemEvent newEvent = new ItemEvent
            {
                ItemId = newItem.Id, // The ID of the item you just created
                RfidTag = newItem.rfid_tag,
                EventType = EventTypes.Created, // The type of event
                EventPayload = null, // No extra payload for creation
            };

            await _context.ItemEvents.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetByTag),
                new { rfidTag = dto.Rfid_tag },
                newItem.MapItemToDto()
            );
        }
        catch (DbUpdateException ex)
        {
            // Check for unique constraint violation (SQLite error code 19)
            if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                return Conflict(ConflictProblem());
            }
            // For other DB errors, return generic error
            return StatusCode(500, new { error = "db_error", message = ex.Message });
        }
    }

    [HttpGet("{rfidTag}")]
    public async Task<IActionResult> GetByTag(string rfidTag)
    {
        var item = await _context
            .item.Include(i => i.OwnerUser)
            .FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        return item == null ? NotFound(NotFoundProblem("Item not found")) : Ok(item.MapItemToDto());
    }

    [HttpPut("{rfidTag}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string rfidTag, UpdateItemDto updates)
    {
        var item = await _context
            .item.Include(i => i.OwnerUser)
            .FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound(NotFoundProblem("Item not found"));

        // Track changes
        var changedProperties = new List<string>();
        if (item.name != updates.Name)
            changedProperties.Add("name");
        if (item.description != updates.Description)
            changedProperties.Add("description");
        if (item.status != updates.Status)
            changedProperties.Add("status");
        if (item.certification_code != updates.CertificationCode)
            changedProperties.Add("certification_code");
        if (item.owner_name != updates.OwnerName)
            changedProperties.Add("owner_name");

        item.UpdateItem(updates);
        await _context.SaveChangesAsync();

        // Serialize changed property names as JSON
        string? payload =
            changedProperties.Count > 0
                ? System.Text.Json.JsonSerializer.Serialize(changedProperties)
                : null;

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            RfidTag = item.rfid_tag,
            EventType = EventTypes.Updated,
            EventPayload = payload,
            ActorEmail = "System",
        };
        await _context.ItemEvents.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        return Ok(item.MapItemToDto());
    }

    [HttpPut("{rfidTag}/owner")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignOwnership(string rfidTag, [FromBody] OwnerAssignDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return UnprocessableEntity(
                new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Validation Error",
                    Detail = "Invalid input data",
                    Extensions = { { "errors", errors } },
                }
            );
        }

        var OwnerEmail = dto.OwnerEmail;
        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound(NotFoundProblem("Item not found"));

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == OwnerEmail);
        if (user == null)
            return NotFound(NotFoundProblem("User not found"));

        item.OwnerUserId = user.Id;
        item.owner_name = user.Name;
        item.last_updated = DateTime.UtcNow;
        item.last_signal = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            RfidTag = item.rfid_tag,
            EventType = EventTypes.OwnershipAssigned,
            EventPayload = System.Text.Json.JsonSerializer.Serialize(
                new { owner_email = user.Email }
            ),
        };
        await _context.ItemEvents.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        return Ok(item.MapItemToDto());
    }

    [HttpDelete("{rfidTag}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string rfidTag)
    {
        var item = await _context
            .item.Include(i => i.OwnerUser)
            .FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound(NotFoundProblem("Item not found"));
        var deletedItemPayload = System.Text.Json.JsonSerializer.Serialize(
            new
            {
                rfid_tag = item.rfid_tag,
                name = item.name,
                owner_email = item.OwnerUser?.Email,
            }
        );
        _context.item.Remove(item);
        await _context.SaveChangesAsync();

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            RfidTag = item.rfid_tag,
            EventType = EventTypes.Deleted,
            EventPayload = deletedItemPayload,
        };
        await _context.ItemEvents.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{rfidTag}/events")]
    public async Task<IActionResult> GetAllEvents(
        string rfidTag,
        [FromQuery] int page = 1,
        [FromQuery(Name = "page_size")] int pageSize = 20
    )
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var query = _context.ItemEvents.Where(e => e.RfidTag == rfidTag).AsQueryable();

        var totalItems = await query.CountAsync();
        var events = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventsDto
            {
                EventType = e.EventType,
                EventPayload = e.EventPayload,
                CreatedAt = e.CreatedAt,
            })
            .ToListAsync();

        var result = new
        {
            page,
            page_size = pageSize,
            total = totalItems,
            events,
        };

        return Ok(result);
    }

    [HttpPost("{rfidTag}/signal")]
    public async Task<ActionResult> UpdateSignal(string rfidTag)
    {
        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound(NotFoundProblem("Item not found"));

        item.last_signal = DateTime.UtcNow;

        var payload = System.Text.Json.JsonSerializer.Serialize(
            new { last_signal = item.last_signal }
        );

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            RfidTag = item.rfid_tag,
            EventType = EventTypes.Signal,
            EventPayload = payload,
            ActorEmail = "System",
        };

        await _context.ItemEvents.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        return Ok(item.MapItemToDto());
    }
}
