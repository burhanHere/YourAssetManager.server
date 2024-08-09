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

        [HttpGet("/GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
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
            ApiResponseDTO result = await _assetManagementRepository.GetAllCategories(userId);
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

        [HttpGet("/GetCategoryById")]
        public async Task<IActionResult> GetCategoryById([FromBody] int AssetCatagoryId)
        {
            ApiResponseDTO result = await _assetManagementRepository.GetCategoryById(AssetCatagoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost("/CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] AssetCatagoryDTO assetCatagoryDTO)
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
            var result = await _assetManagementRepository.CreateCategory(userId, assetCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            return BadRequest(result);
        }

        [HttpPut("/UpdateAssetCategory")]
        public async Task<IActionResult> UpdateAssetCategory([FromBody] int AssetCatagoryId, [FromBody] AssetCatagoryDTO assetCatagoryDTO)
        {
            ApiResponseDTO result = await _assetManagementRepository.UpdateAssetCategory(AssetCatagoryId, assetCatagoryDTO);
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

        [HttpDelete("/DeleteAssetCategory")]
        public async Task<IActionResult> DeleteAssetCategory(int AssetCatagoryId)
        {
            ApiResponseDTO result = await _assetManagementRepository.DeleteAssetCatagory(AssetCatagoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status403Forbidden)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return BadRequest(result);
        }

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