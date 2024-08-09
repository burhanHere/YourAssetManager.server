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

        [HttpGet("/GetAssetManagers")]
        public async Task<IActionResult> GetAssetManagers()
        {
            ApiResponceDTO result = await _assetManagementRepository.GetAssetManagers(ClaimTypes.Name);
            if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("/CreateNewUser")]
        public async Task<IActionResult> CreateNewUser(NewUserDTO employeeDTO)//panding
        {
            ApiResponceDTO result = await _assetManagementRepository.CreateNewUser(ClaimTypes.Name, employeeDTO);
            if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // [HttpPut("/api/asset-managers/{id}")]
        // public async Task<IActionResult> UpdateAssetManager(int id) { return Ok(); }

        // [HttpDelete("/api/asset-managers/{id}")]
        // public async Task<IActionResult> DeleteAssetManager(int id) { return Ok(); }

        [HttpGet("/GetAssetCategories")]
        public async Task<IActionResult> GetAssetCategories()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponceDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponceData = new List<string> { "User not found in token." }
                });
            }
            ApiResponceDTO result = await _assetManagementRepository.GetAssetCategories(userName);
            return Ok(result);
        }

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