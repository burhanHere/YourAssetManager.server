using Microsoft.AspNetCore.Identity;
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
            var targetUser = await _userManager.FindByIdAsync(currentLogedInUser);
            if (targetUser == null)
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

            AssetRequest newRequest = new()
            {
                RequestDescription = assetRequestDTO.RequestDescription,
                RequestStatus = "PENDING",
                RequesterId = targetUser.Id
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