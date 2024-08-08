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
        private readonly AssetManagementRepository _assetManagementRepository = new(userManager, applicationDbContext);

        [HttpGet("/api/asset-managers")]
        public async Task<IActionResult> GetAssetManagers()
        {
            ApiResponceDTO result = await _assetManagementRepository.GetAssetManagers(ClaimTypes.Name);
            return Ok();
        }

        // [HttpPost("/api/asset-managers")]
        // public async Task<IActionResult> CreateAssetManager() { return Ok(); }

        // [HttpPut("/api/asset-managers/{id}")]
        // public async Task<IActionResult> UpdateAssetManager(int id) { return Ok(); }

        // [HttpDelete("/api/asset-managers/{id}")]
        // public async Task<IActionResult> DeleteAssetManager(int id) { return Ok(); }

        // [HttpGet("/api/asset-categories")]
        // public async Task<IActionResult> GetAssetCategories() { return Ok(); }

        // [HttpPost("/api/asset-categories")]
        // public async Task<IActionResult> CreateAssetCategory() { return Ok(); }

        // [HttpPut("/api/asset-categories/{id}")]
        // public async Task<IActionResult> UpdateAssetCategory(int id) { return Ok(); }

        // [HttpDelete("/api/asset-categories/{id}")]
        // public async Task<IActionResult> DeleteAssetCategory(int id) { return Ok(); }

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