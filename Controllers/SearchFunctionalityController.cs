using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class SearchFunctionalityController : ControllerBase
    {
        [HttpGet("/api/assets/search")]
        public IActionResult SearchAssets() { return Ok(); }

    }
}