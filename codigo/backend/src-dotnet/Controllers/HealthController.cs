using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET /health
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok("Service is running.");
        }
    }
}
