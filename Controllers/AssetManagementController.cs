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

        [HttpGet("/GetAssetCategories")]
        public async Task<IActionResult> GetAssetCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _assetManagementRepository.GetAssetCategories(userId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpPost("CreateAssetCategory")]
        public async Task<IActionResult> CreateAssetCategory(AssetCatagoryDTO assetCatagoryDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(userId))
                {
                    // If the username is not found, return an unauthorized response
                    return Unauthorized(new ApiResponseDTO
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        ResponseData = new List<string> { "User not found in token." }
                    });
                }
            }
            var result = await _assetManagementRepository.CreateAssetCategory(userId, assetCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

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