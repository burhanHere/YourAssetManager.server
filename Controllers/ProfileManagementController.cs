using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class ProfileManagementController : ControllerBase
    {
        [HttpGet("/api/profile")]
        public IActionResult GetProfile() {return Ok();}

        [HttpPut("/api/profile")]
        public IActionResult UpdateProfile() {return Ok();}

    }
}