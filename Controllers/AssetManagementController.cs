using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class AssetManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : ControllerBase
    {


        // [HttpGet("/api/assets")]
        // public async Task<IActionResult> GetAssets() { return Ok(); }

        // [HttpPost("/api/assets")]
        // public async Task<IActionResult> CreateAsset() { return Ok(); }

        // [HttpPut("/api/assets/{id}")]
        // public async Task<IActionResult> UpdateAsset(int id) { return Ok(); }

        // [HttpDelete("/api/assets/{id}")]
        // public async Task<IActionResult> DeleteAsset(int id) { return Ok(); }

        // [HttpPut("/api/assets/{id}/status")]
        // public async Task<IActionResult> UpdateAssetStatus(int id) { return Ok(); }

        // [HttpGet("/api/assets/statistics")]
        // public async Task<IActionResult> GetAssetStatistics() { return Ok(); }



    }
}