using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RfidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private static readonly string[] value = ["user"];

        // GET: api/debug/auth
        [HttpGet("auth")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult<IActionResult>(
                Ok(new { user = "dummy@test.com", roles = value })
            );
        }
    }
}
