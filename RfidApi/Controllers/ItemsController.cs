using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RfidApi.Data;
using RfidApi.Models;
using RfidApi.Models.Dtos;

namespace RfidApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context) => _context = context;

    // GET: api/items
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? ownerEmail = null
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

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(item => item.Id)
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
    public async Task<IActionResult> Add([FromBody] CreateItemDto dto)
    {
        Item newItem = dto.MapDtoToItem();
        await _context.item.AddAsync(newItem);
        try
        {
            await _context.SaveChangesAsync();

            // Create Item event before return

            ItemEvent newEvent = new ItemEvent
            {
                ItemId = newItem.Id, // The ID of the item you just created
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
                return Conflict(new { error = "conflict", message = "rfid_tag already exists" });
            }
            // For other DB errors, return generic error
            return StatusCode(500, new { error = "db_error", message = ex.Message });
        }
    }

    [HttpGet("{rfidTag}")]
    public async Task<IActionResult> GetByTag(string rfidTag)
    {
        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        return item == null ? NotFound() : Ok(item.MapItemToDto());
    }

    [HttpPut("{rfidTag}")]
    public async Task<IActionResult> Update(string rfidTag, UpdateItemDto updates)
    {
        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound();

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
            EventType = EventTypes.Updated,
            EventPayload = payload,
        };
        await _context.ItemEvents.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        return Ok(item.MapItemToDto());
    }

    [HttpPut("{rfidTag}/owner")]
    public async Task<IActionResult> AssignOwnership(string rfidTag, [FromBody] OwnerAssignDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return UnprocessableEntity(new { error = "validation_error", details = errors });
        }

        var OwnerEmail = dto.OwnerEmail;
        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound(new { error = "not_found", message = "Item not found" });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == OwnerEmail);
        if (user == null)
            return NotFound(new { error = "not_found", message = "User not found" });

        item.OwnerUserId = user.Id;
        item.owner_name = user.Name;
        item.last_updated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
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
    public async Task<IActionResult> Delete(string rfidTag)
    {
        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound();
        _context.item.Remove(item);
        await _context.SaveChangesAsync();

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            EventType = EventTypes.Deleted,
            EventPayload = null,
        };
        await _context.ItemEvents.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{rfidTag}/events")]
    public async Task<IActionResult> GetAllEvents(
        string rfidTag,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var item = await _context.item.FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound(new { error = "not_found", message = "Item not found" });

        var query = _context.ItemEvents.Where(e => e.ItemId == item.Id).AsQueryable();

        var totalItems = await query.CountAsync();
        var events = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new
            {
                event_type = e.EventType,
                event_payload = e.EventPayload,
                created_at = e.CreatedAt,
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
}
