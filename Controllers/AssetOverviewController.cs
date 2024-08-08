using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]

    public class AssetOverviewController : ControllerBase
    {
        [HttpGet("/api/asset-overview")]
        public IActionResult GetAssetOverview() { return Ok(); }

    }
}