using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class AssetManagementController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly AssetManagementRepository _assetManagementRepository = new(applicationDbContext, userManager);

        [HttpGet("/GetAllAssets")]
        public async Task<ApiResponseDTO> GetAllAssets()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // If the username is not found, return an unauthorized response
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            ApiResponseDTO result = await _assetManagementRepository.GetAllAssets(userId);
            return result;
        }

        // [HttpGet("/GetAssetById")]
        // public async Task<IActionResult> GetAssetById() { return Ok(); }

        // [HttpPost("/CreateAsset")]
        // public async Task<IActionResult> CreateAsset() { return Ok(); }

        // [HttpPut("/UpdateAsset")]
        // public async Task<IActionResult> UpdateAsset(int id) { return Ok(); }

        // [HttpDelete("/DeleteAsset")]
        // public async Task<IActionResult> DeleteAsset(int id) { return Ok(); }

        // [HttpPut("/UpdateAssetStatusById")]
        // public async Task<IActionResult> UpdateAssetStatus(int id) { return Ok(); }

        // [HttpGet("GetAssetStatistics")]
        // public async Task<IActionResult> GetAssetStatistics() { return Ok(); }

        // [HttpPost("/AssignAsset")]
        // public async Task<IActionResult> AssignAsset([FromBody] AssetRequestDTO request) { return Ok(); }

        // [HttpPost("/UnassignAsset")]
        // public async Task<IActionResult> UnassignAsset([FromBody] AssetRequestDTO request) { return Ok(); }

        // [HttpGet("/GetAssetsAssignedToUser/{userId}")]
        // public async Task<IActionResult> GetAssetsAssignedToUser(int userId) { return Ok(); }

        // [HttpGet("/GetAssetAssignmentDetails/{assetId}")]
        // public async Task<IActionResult> GetAssetAssignmentDetails(int assetId) { return Ok(); }

        // [HttpPut("/UpdateAssetAssignment")]
        // public async Task<IActionResult> UpdateAssetAssignment([FromBody] AssetRequestDTO request) { return Ok(); }
    }
}