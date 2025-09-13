using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 20;

        var totalItems = _context.item.Count();
        var items = _context
            .item.OrderBy(item => item.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => item.MapItemToDto())
            .ToList();

        var result = new
        {
            page,
            pageSize,
            totalItems,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            items,
        };

        return Ok(result);
    }

    [HttpPost]
    public IActionResult Add([FromBody] CreateItemDto dto)
    {
        Item newItem = dto.MapDtoToItem();
        _context.item.Add(newItem);
        try
        {
            _context.SaveChanges();

            // Create Item event before return

            ItemEvent newEvent = new ItemEvent
            {
                ItemId = newItem.Id, // The ID of the item you just created
                EventType = "created", // The type of event
                EventPayload = null, // No extra payload for creation
            };

            _context.ItemEvents.Add(newEvent);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetByTag),
                new { rfidTag = dto.RfidTag },
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
    public IActionResult GetByTag(string rfidTag)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
        return item == null ? NotFound() : Ok(item.MapItemToDto());
    }

    [HttpPut("{rfidTag}")]
    public IActionResult Update(string rfidTag, UpdateItemDto updates)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
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
        _context.SaveChanges();

        // Serialize changed property names as JSON
        string? payload =
            changedProperties.Count > 0
                ? System.Text.Json.JsonSerializer.Serialize(changedProperties)
                : null;

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            EventType = "updated",
            EventPayload = payload,
        };
        _context.ItemEvents.Add(newEvent);
        _context.SaveChanges();

        return Ok(item.MapItemToDto());
    }

    [HttpDelete("{rfidTag}")]
    public IActionResult Delete(string rfidTag)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound();
        _context.item.Remove(item);
        _context.SaveChanges();

        ItemEvent newEvent = new ItemEvent
        {
            ItemId = item.Id,
            EventType = "deleted",
            EventPayload = null,
        };
        _context.ItemEvents.Add(newEvent);
        _context.SaveChanges();

        return NoContent();
    }
}
