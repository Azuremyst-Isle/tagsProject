using Microsoft.AspNetCore.Mvc;
using RfidApi.Data;
using RfidApi.Models;

namespace RfidApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context) => _context = context;

    [HttpGet]
    public IActionResult GetAll() => Ok(_context.item.ToList());

    [HttpPost]
    public IActionResult Add(Item item)
    {
        _context.item.Add(item);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetByTag), new { rfidTag = item.rfid_tag }, item);
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
