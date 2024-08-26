using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetActionsManagementRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
    {
        private readonly ApplicationDbContext _applicationRepository = applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApiResponseDTO> RequestAsset(string currentLogedInUser, AssetRequestDTO assetRequestDTO)
        {
            var targetUserOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == currentLogedInUser);
            if (targetUserOrganization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Target user not found!"
                    }
                };
            }
            if (string.IsNullOrEmpty(assetRequestDTO.RequestDescription))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Request Description cant be empty!"
                    }
                };
            }
            AssetRequest newRequest = new()
            {
                RequestDescription = assetRequestDTO.RequestDescription,
                RequestStatus = "PENDING",
                RequesterId = targetUserOrganization.UserId,
                OrganizationId = targetUserOrganization.OrganizationId
            };

            _applicationRepository.AssetRequests.Add(newRequest);
            var savedDbChanges = await _applicationRepository.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>()
                    {
                        "Failed to Create Asset Request."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>()
                    {
                        "Asset Request created successfully."
                    }
            };
        }

    }
}