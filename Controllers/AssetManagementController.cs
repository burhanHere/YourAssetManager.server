using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/[controller]")]
    public class AssetManagementController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly AssetManagementRepository _assetManagementRepository = new(applicationDbContext, userManager);

        [HttpPost("CreateAsset")]
        public async Task<IActionResult> CreateAsset([FromBody] AssetDTO newAssetDTO)
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
            var result = await _assetManagementRepository.CreateAsset(userId, newAssetDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpPut("UpdateAsset")]
        public async Task<IActionResult> UpdateAsset([FromBody] AssetDTO newAssetDTO)
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

            var result = await _assetManagementRepository.UpdateAsset(userId, newAssetDTO);
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

        [HttpGet("GetAllAssets")]
        public async Task<ApiResponseDTO> GetAllAssets()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(userId))
                {
                    // If the username is not found, return an unauthorized response
                    return new ApiResponseDTO
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        ResponseData = new List<string> { "User not found in token." }
                    };
                }
            }
            var result = await _assetManagementRepository.GetAllAssets(userId);
            return result;
        }

        [HttpGet("GetAssetById")]
        public async Task<ApiResponseDTO> GetAssetById(int assetId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(userId))
                {
                    // If the username is not found, return an unauthorized response
                    return new ApiResponseDTO
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        ResponseData = new List<string> { "User not found in token." }
                    };
                }
            }
            var result = await _assetManagementRepository.GetAssetsById(userId, assetId);
            return result;
        }

        [HttpDelete("DeleteAsset")]
        public async Task<IActionResult> DeleteAsset(int assetId)
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
            var result = await _assetManagementRepository.DeleteAsset(userId, assetId);
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

        // [HttpPost("AssignAsset")]
        // public async Task<IActionResult> AssignAsset([FromBody] AssetRequestDTO request) { return Ok(); }

        // [HttpPost("UnassignAsset")]
        // public async Task<IActionResult> UnassignAsset([FromBody] AssetRequestDTO request) { return Ok(); }

        // [HttpGet("GetAssetsAssignedToUser/{userId}")]
        // public async Task<IActionResult> GetAssetsAssignedToUser(int userId) { return Ok(); }

        // [HttpGet("GetAssetAssignmentDetails/{assetId}")]
        // public async Task<IActionResult> GetAssetAssignmentDetails(int assetId) { return Ok(); }

        // [HttpPut("UpdateAssetAssignment")]
        // public async Task<IActionResult> UpdateAssetAssignment([FromBody] AssetRequestDTO request) { return Ok(); }
    }
}