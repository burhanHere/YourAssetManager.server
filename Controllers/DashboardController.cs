using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]

    public class DashboardController : ControllerBase
    {
        [HttpGet("/api/dashboard/organization-owner")]
        public IActionResult GetOrganizationOwnerDashboard() { return Ok(); }

        [HttpGet("/api/dashboard/asset-manager")]
        public IActionResult GetAssetManagerDashboard() { return Ok(); }

    }
}