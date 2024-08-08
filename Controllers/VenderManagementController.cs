using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class VenderManagementController : ControllerBase
    {
        [HttpGet("/api/vendors")]
        public IActionResult GetVendors() { return Ok(); }

        [HttpPost("/api/vendors")]
        public IActionResult CreateVendor() { return Ok(); }

        [HttpPut("/api/vendors/{id}")]
        public IActionResult UpdateVendor(int id) { return Ok(); }

        [HttpDelete("/api/vendors/{id}")]
        public IActionResult DeleteVendor(int id) { return Ok(); }

    }
}