using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class Audit : ControllerBase
    {
        private readonly AppDbContext _context;

        public Audit(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("items/{rfidTag}")]
        [Authorize(Roles = "user,admin")]
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

        [HttpGet("items/{rfidTag}/CSV")]
        [AllowAnonymous]
        public async Task<IActionResult> produceCSV(string rfidTag)
        {
            var events = await _context
                .ItemEvents.Where(e => e.RfidTag == rfidTag)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new
                {
                    e.CreatedAt,
                    e.EventType,
                    e.EventPayload,
                })
                .ToListAsync();

            if (events.Count == 0)
                return NotFound(NotFoundProblem("No events found for this RFID tag"));

            // Create CSV
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("created_at,event_type,event_payload");
            foreach (var e in events)
            {
                var createdAt = e.CreatedAt.ToString("o"); // ISO 8601 format
                var eventType = e.EventType.Replace(",", " "); // Avoid commas in CSV
                var eventPayload = e.EventPayload?.Replace(",", " ") ?? "null"; // Avoid commas and handle nulls
                csv.AppendLine($"{createdAt},{eventType},{eventPayload}");
            }

            return File(
                System.Text.Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                $"events_{rfidTag}.csv"
            );
        }
    }
}
