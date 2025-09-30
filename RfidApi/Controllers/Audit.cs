using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RfidApi.Data;
using RfidApi.Models;
using RfidApi.Models.Dtos;
using static RfidApi.Errors.CustomErrorHandlers;

namespace RfidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Audit : ControllerBase
    {
        private readonly AppDbContext _context;

        public Audit(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("items/{rfidTag}")]
        public async Task<IActionResult> Get(string rfidTag)
        {
            var item = await _context
                .item.Include(i => i.OwnerUser)
                .FirstOrDefaultAsync(i => i.rfid_tag == rfidTag);
            if (item == null)
            {
                return NotFound(NotFoundProblem("Item not found"));
            }

            // Search of events
            var queryEvent = _context.ItemEvents.Where(e => e.RfidTag == rfidTag).AsQueryable();

            var events = await queryEvent
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EventsDto
                {
                    EventType = e.EventType,
                    EventPayload = e.EventPayload,
                    CreatedAt = e.CreatedAt,
                })
                .ToListAsync();

            var result = new { item = item.MapItemToDto(), events };

            return Ok(result);
        }
    }
}
