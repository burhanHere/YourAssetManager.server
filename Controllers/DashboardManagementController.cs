using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Policy = "RequireOrganizationOwnerOrAssetManagerAccess")]
    [ApiController]
    [Route("YourAssetManager.Server/[controller]")]
    public class DashboardManagementController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly DashboardManagementRepository _dashboardManagementRepository = new(applicationDbContext, userManager);

        [HttpGet("GetDashBoardStatiticsData")]
        public async Task<ApiResponseDTO> GetDashBoardStatiticsData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            ApiResponseDTO result = await _dashboardManagementRepository.GetDashBoardStatiticsData(userId);
            return result;
        }

        [HttpGet("GetAllAssetRequests")]
        public async Task<ApiResponseDTO> GetAllAssetRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            ApiResponseDTO result = await _dashboardManagementRepository.GetAllAssetRequests(userId);
            return result;
        }

    }
}