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
    [ApiController]
    [Route("YourAssetManager.Server/[controller]")]
    public class AssetActionsManagementController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, MailSettingsDTO mailSettings) : ControllerBase
    {
        private readonly AssetActionsManagementRepository _assetActionsManagementRepository = new(applicationDbContext, userManager, mailSettings);

        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerEmployeeAccess")]
        [HttpPost("RequestAsset")]
        public async Task<IActionResult> RequestAsset(AssetRequestDTO assetRequestDTO)
        {
            var currectLogedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currectLogedInUserId))
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _assetActionsManagementRepository.RequestAsset(currectLogedInUserId, assetRequestDTO);
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

        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        [HttpPost("ProcessAssetRequest")]
        public async Task<IActionResult> ProcessAssetRequest(int requestId, bool action)
        {
            var currectLogedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currectLogedInUserId))
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _assetActionsManagementRepository.ProcessAssetRequest(currectLogedInUserId, requestId, action);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        [HttpPost("AssignAsset")]
        public async Task<IActionResult> AssignAsset(AssetAssignmentDTO assetAssignmentDTO)
        {
            var currectLogedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currectLogedInUserId))
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _assetActionsManagementRepository.AssetAssign(currectLogedInUserId, assetAssignmentDTO);
            return Ok(result);
        }
    }
}