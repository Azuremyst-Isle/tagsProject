using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RfidApi.Data;
using RfidApi.Models;

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
        var items = _context.item.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
    public IActionResult Add(Item item)
    {
        _context.item.Add(item);
        try
        {
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetByTag), new { rfidTag = item.rfid_tag }, item);
        }
        catch (DbUpdateException ex)
        {
            // Check for unique constraint violation (SQLite error code 19)
            if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                return Conflict(new { error = "conflict", message = "rfid_tag already exists" });
            }
            // For other DB errors, return generic error
            return StatusCode(
                500,
                new { error = "db_error", message = "A database error occurred" }
            );
        }
    }

    [HttpGet("{rfidTag}")]
    public IActionResult GetByTag(string rfidTag)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPut("{rfidTag}")]
    public IActionResult Update(string rfidTag, Item updated)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound();
        item.name = updated.name;
        item.description = updated.description;
        item.status = updated.status;
        item.certification_code = updated.certification_code;
        item.owner_name = updated.owner_name;
        item.last_updated = DateTime.UtcNow;
        _context.SaveChanges();
        return Ok(item);
    }

    [HttpDelete("{rfidTag}")]
    public IActionResult Delete(string rfidTag)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound();
        _context.item.Remove(item);
        _context.SaveChanges();
        return NoContent();
    }
}
