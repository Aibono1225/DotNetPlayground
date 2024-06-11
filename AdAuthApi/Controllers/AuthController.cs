using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("windows-auth")]
        [Authorize]
        public IActionResult Get()
        {
            var userName = User.Identity.Name;
            return Ok(new { Message = "Authenticated", User = userName });
        }
    }
}
