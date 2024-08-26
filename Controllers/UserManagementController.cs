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
        public async Task<IActionResult> AppointAssetManager([FromBody] UserManagementDTO Appointee)
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
            ApiResponseDTO result = await _userManagementRepository.AppointAssetManager(currectLogedInUserId, Appointee.Id);
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
        public async Task<IActionResult> DismissAssetManager([FromBody] UserManagementDTO Appointee)
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
            ApiResponseDTO result = await _userManagementRepository.DismissAssetManager(currectLogedInUserId, Appointee.Id);
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
        public async Task<IActionResult> DeactivateAccount([FromBody] UserManagementDTO targetUser)
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
            if (currectLogedInUserId == targetUser.Id)
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
            ApiResponseDTO result = await _userManagementRepository.DeactivateAccount(currectLogedInUserId, targetUser.Id);
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
        public async Task<IActionResult> ActivateAccount([FromBody] UserManagementDTO targetUser)
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
            if (currectLogedInUserId == targetUser.Id)
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
            ApiResponseDTO result = await _userManagementRepository.ActivateAccount(currectLogedInUserId, targetUser.Id);
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

        [HttpGet("GetAllUser")]
        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        public async Task<ApiResponseDTO> GetAllUser()
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
            ApiResponseDTO result = await _userManagementRepository.GetAllUsers(currectLogedInUserId);
            return result;
        }

        [HttpGet("GetUserById")]
        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
        public async Task<ApiResponseDTO> GetUserById(UserManagementDTO targetUser)
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
            ApiResponseDTO result = await _userManagementRepository.GetUserById(currectLogedInUserId, targetUser.Id);
            return result;
        }

        [HttpPut("UpdateUser")]
        [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerEmployeeAccess")]
        public async Task<ApiResponseDTO> UpdateUserProfile([FromBody] UserProfileUpdateDTO userDTO)
        {
            return new ApiResponseDTO();
        }
    }
}