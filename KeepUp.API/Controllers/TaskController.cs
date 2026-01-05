using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        [HttpGet("secure")]
        [Authorize]
        public IActionResult Secure()
        {
            return Ok("You are authenticated");
        }
    }
}
