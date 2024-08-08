using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class LogActionsController : ControllerBase
    {
        [HttpGet("/api/logs")]
        public IActionResult GetLogActions() { return Ok(); }

    }
}