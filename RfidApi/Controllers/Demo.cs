using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RfidApi.Data;

namespace RfidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Demo : ControllerBase
    {
        private readonly AppDbContext _context;

        public Demo(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/demo/summary
        [HttpGet("summary")]
        public async Task<IActionResult> Summary()
        {
            var itemStats = await _context
                .item.GroupBy(i => i.OwnerUserId != null)
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
                    }
                )
            );
        }
    }
}
