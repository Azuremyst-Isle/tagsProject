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
            .Select(item => new ItemDto(
                item.rfid_tag,
                item.name,
                item.description,
                item.status,
                item.certification_code,
                item.owner_name,
                item.last_updated
            ))
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
        return item == null
            ? NotFound()
            : Ok(
                new ItemDto(
                    item.rfid_tag,
                    item.name,
                    item.description,
                    item.status,
                    item.certification_code,
                    item.owner_name,
                    item.last_updated
                )
            );
    }

    [HttpPut("{rfidTag}")]
    public IActionResult Update(string rfidTag, UpdateItemDto updates)
    {
        var item = _context.item.FirstOrDefault(i => i.rfid_tag == rfidTag);
        if (item == null)
            return NotFound();

        item.UpdateItem(updates);
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
        return NoContent();
    }
}
