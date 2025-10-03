using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RfidApi.Data;
using RfidApi.Models;
using static RfidApi.Errors.CustomErrorHandlers;

namespace RfidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Demo : ControllerBase
    {
        private readonly AppDbContext _context;

        public Demo(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/demo/summary
        [HttpGet("summary")]
        [AllowAnonymous]
        public async Task<IActionResult> Summary()
        {
            var ItemsQuery = _context.item.AsQueryable();

            var OnlineItemsCount = await ItemsQuery
                .Where(i => i.last_signal >= DateTime.UtcNow.AddMinutes(-5))
                .CountAsync();

            var itemStats = await ItemsQuery
                .GroupBy(i => i.OwnerUserId != null)
                .Select(g => new
                {
                    Owned = g.Key ? g.Count() : 0,
                    Unowned = !g.Key ? g.Count() : 0,
                    Total = g.Count(),
                })
                .ToListAsync();

            var userCount = await _context.Users.CountAsync();
            var eventCount = await _context.ItemEvents.CountAsync();

            var itemCount = itemStats.Sum(s => s.Total);
            var ownedItemCount = itemStats.Sum(s => s.Owned);
            var unownedItemCount = itemStats.Sum(s => s.Unowned);

            return await Task.FromResult<IActionResult>(
                Ok(
                    new
                    {
                        total_items = itemCount,
                        total_users = userCount,
                        total_events = eventCount,
                        items_with_owner = ownedItemCount,
                        items_without_owner = unownedItemCount,
                        items_online = OnlineItemsCount,
                    }
                )
            );
        }

        // POST: api/demo/reset
        [HttpPost("reset")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Reset()
        {
            // Clear tables
            _context.ItemEvents.RemoveRange(_context.ItemEvents);
            _context.item.RemoveRange(_context.item);
            _context.Users.RemoveRange(_context.Users);

            await _context.SaveChangesAsync();

            // Seed values
            var userList = new List<Users>
            {
                new Users
                {
                    Name = "Alice",
                    Email = "alice@example.com",
                    Role = "admin",
                },
                new Users
                {
                    Name = "Bob",
                    Email = "bob@example.com",
                    Role = "user",
                },
                new Users
                {
                    Name = "Clara",
                    Email = "clara@example.com",
                    Role = "user",
                },
            };

            await _context.Users.AddRangeAsync(userList);

            int totalItems = 8;

            var itemList = new List<Item>();
            var EventList = new List<ItemEvent>();

            for (int i = 1; i <= totalItems; i++)
            {
                itemList.Add(
                    new Item
                    {
                        rfid_tag = (100 + i).ToString(),
                        name = $"Demo Item {i}",
                        description = $"demo item number {i}",
                        status = i % 2 == 0 ? "available" : "unavailable",
                    }
                );
            }

            await _context.item.AddRangeAsync(itemList);
            await _context.SaveChangesAsync();

            for (int i = 0; i < totalItems; i++)
            {
                EventList.Add(
                    new ItemEvent
                    {
                        ItemId = itemList[i].Id, // The ID of the item you just created
                        RfidTag = itemList[i].rfid_tag,
                        EventType = EventTypes.Created, // The type of event
                        EventPayload = null, // No extra payload for creation
                    }
                );
            }

            // assign ownership

            for (int i = 0; i < totalItems; i++)
            {
                if (i % 2 == 0) // Assign owner to even indexed items
                {
                    // Assign users name and Id in round-robin fashion
                    itemList[i].owner_name = userList[i % userList.Count].Name;
                    itemList[i].OwnerUserId = userList[i % userList.Count].Id;

                    EventList.Add(
                        new ItemEvent
                        {
                            ItemId = itemList[i].Id,
                            RfidTag = itemList[i].rfid_tag,
                            EventType = EventTypes.OwnershipAssigned,
                            EventPayload = System.Text.Json.JsonSerializer.Serialize(
                                new { owner_email = userList[i % userList.Count].Email }
                            ),
                        }
                    );
                }
            }

            await _context.ItemEvents.AddRangeAsync(EventList);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Database reset and seeded." });
        }

        [HttpPost("fill-events/{rfidTag}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> FillEvents(
            string rfidTag,
            [FromQuery(Name = "number")] int eventNumber = 1
        )
        {
            if (eventNumber < 1)
                eventNumber = 1;

            var Events = await _context.ItemEvents.FirstOrDefaultAsync(i => i.RfidTag == rfidTag);

            if (Events == null)
            {
                return NotFound(NotFoundProblem("Item Events not found"));
            }

            var eventTypes = new[]
            {
                EventTypes.Created,
                EventTypes.Updated,
                EventTypes.Deleted,
                EventTypes.Signal,
            };

            var random = new Random();

            for (int i = 0; i < eventNumber; i++)
            {
                var eventType = eventTypes[random.Next(eventTypes.Length)];
                string? payload = eventType switch
                {
                    EventTypes.Created => null,
                    EventTypes.Updated => "[\"name\",\"status\"]",
                    EventTypes.Deleted => null,
                    EventTypes.Signal => System.Text.Json.JsonSerializer.Serialize(
                        new { last_signal = DateTime.UtcNow }
                    ),
                    _ => null,
                };
                var newEvent = new ItemEvent
                {
                    ItemId = Events.ItemId,
                    RfidTag = rfidTag,
                    EventType = eventType,
                    EventPayload = payload,
                };
                await _context.ItemEvents.AddAsync(newEvent);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Events Generated" });
        }
    }
}
