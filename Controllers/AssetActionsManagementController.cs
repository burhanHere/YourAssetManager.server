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
    public class AssetActionsManagementController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly AssetActionsManagementRepository _assetActionsManagementRepository = new(applicationDbContext, userManager);

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

        // public async Task<IActionResult> AcceptAssetRequest()
        // {

        // }
    }
}