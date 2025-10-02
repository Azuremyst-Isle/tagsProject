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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context) => _context = context;

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20
        )
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 20;

            var total = await _context.Users.CountAsync();
            var users = await _context
                .Users.OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => u.MapUserToDto())
                .ToListAsync();

            var result = new
            {
                users,
                page,
                pageSize,
                total,
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        { // Create user
            Users newUser = userDto.MapDtoToUser();
            await _context.Users.AddAsync(newUser);

            try
            {
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetUsers),
                    new { id = newUser.Id },
                    newUser.MapUserToDto()
                );
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("UNIQUE constraint failed:") == true)
                {
                    return Conflict(ConflictProblem("A user with this email already exists."));
                }

                // For other DB errors, return generic error
                return StatusCode(500, new { error = "db_error", message = ex.Message });
            }
        }
    }
}
