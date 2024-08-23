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
    public class UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext) : ControllerBase
    {
        private readonly UserManagementRepository _userManagementRepository = new(userManager, roleManager, applicationDbContext);

        [HttpPost("AssignAssetManager")]
        [Authorize(Policy = "RequireOrganizationOwnerAccess")]
        public async Task<IActionResult> AppointAssetManager(string AppointeeId)
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
            ApiResponseDTO result = await _userManagementRepository.AppointAssetManager(currectLogedInUserId, AppointeeId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return BadRequest(result);
        }

        [HttpPost("DismissAssetManager")]
        [Authorize(Policy = "RequireOrganizationOwnerAccess")]
        public async Task<IActionResult> DismissAssetManager(string AppointeeId)
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
            ApiResponseDTO result = await _userManagementRepository.DismissAssetManager(currectLogedInUserId, AppointeeId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return BadRequest(result);
        }

        [HttpPost("DeactivateAccount")]
        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        public async Task<IActionResult> DeactivateAccount(string targetUserId)
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
            if (currectLogedInUserId == targetUserId)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Currect LogedIn User and target user cant be equal."
                    }
                });
            }
            ApiResponseDTO result = await _userManagementRepository.DeactivateAccount(currectLogedInUserId, targetUserId);
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

        [HttpPost("ActivateAccount")]
        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        public async Task<IActionResult> ActivateAccount(string targetUserId)
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
            if (currectLogedInUserId == targetUserId)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Currect LogedIn User and target user cant be equal."
                    }
                });
            }
            ApiResponseDTO result = await _userManagementRepository.ActivateAccount(currectLogedInUserId, targetUserId);
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
    }
}