using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RfidApi.Data;
using RfidApi.Models;

namespace RfidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context) => _context = context;

        // GET: api/users
        [HttpGet]
        public IActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 20;

            var totalUsers = _context.Users.Count();
            var users = _context
                .Users.OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => u.MapUserToDto())
                .ToList();

            var result = new
            {
                page,
                pageSize,
                totalUsers,
                totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                users,
            };

            return Ok(result);
        }
    }
}
