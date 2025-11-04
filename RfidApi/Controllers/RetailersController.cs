using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RfidApi.Data;
using RfidApi.Models;

namespace RfidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetailersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RetailersController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var retailers = await _context.Retailers.ToListAsync();
            return Ok(retailers);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Retailers retailer)
        {
            _context.Retailers.Add(retailer);
            await _context.SaveChangesAsync();
            return Ok(
                new
                {
                    name = retailer.Name,
                    identifier = retailer.Identifier,
                    email = retailer.Email,
                }
            );
        }
    }
}
