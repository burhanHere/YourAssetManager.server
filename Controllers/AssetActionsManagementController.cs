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
        public async Task<IActionResult> RequestAsset([FromBody] AssetRequestDTO assetRequestDTO)
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
        public async Task<IActionResult> ProcessAssetRequest([FromBody] AssetRequestProcessDTO assetRequestProcessDTO)
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
            ApiResponseDTO result = await _assetActionsManagementRepository.ProcessAssetRequest(currectLogedInUserId, assetRequestProcessDTO.RequestId, assetRequestProcessDTO.Action);
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
        public async Task<IActionResult> AssignAsset([FromBody] AssetAssignmentDTO assetAssignmentDTO)
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

        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerEmployeeAccess")]
        [HttpGet("GetAssetRequestsByUserId")]
        public async Task<ApiResponseDTO> GetAssetRequestsByUserId()
        {
            var currectLogedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currectLogedInUserId))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }

            ApiResponseDTO result = await _assetActionsManagementRepository.GetAssetRequestsByUserId(currectLogedInUserId);
            return result;
        }

        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        [HttpPost("ReturnAsset")]
        public async Task<IActionResult> ReturnAsset([FromBody] AssetReturnDTO assetReturnDTO)
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
            ApiResponseDTO result = await _assetActionsManagementRepository.ReturnAsset(currectLogedInUserId, assetReturnDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerEmployeeAccess")]
        [HttpPost("CancelRequestAsset")]
        public async Task<IActionResult> CancelRequestAsset(int reqiestId)
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

            ApiResponseDTO result = await _assetActionsManagementRepository.CancelRequestAsset(currectLogedInUserId, reqiestId);
            return Ok(result);
        }
    }
}