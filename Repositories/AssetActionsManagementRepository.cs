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
        public async Task<ApiResponseDTO> DeclineAssetRequest(string currentLogedInUser, int RequestId)
        {
            var targetUser = await _userManager.FindByIdAsync(currentLogedInUser);
            if (targetUser == null)
            {
                // Return error if user not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }

            var userOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == targetUser.Id && x.Organization.ActiveOrganization);
            if (userOrganization == null)
            {

                // Return error if organization and user association not found or if organization is deleted(deactivated)
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can perform this action as organization is not active or associated to this user.",
                    }
                };
            }

            AssetRequest targetAssetRequest = await _applicationRepository.AssetRequests.FirstOrDefaultAsync(x => x.OrganizationId == userOrganization.OrganizationId && x.Id == RequestId);
            if (targetAssetRequest == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Target Request not found."
                    }
                };
            }
            if (targetAssetRequest.RequestStatus != "PENDING")
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't perform this action. Request Status Not pending.",
                    }
                };
            }
            targetAssetRequest.RequestStatus = "DECLINED";
            _applicationRepository.AssetRequests.Update(targetAssetRequest);
            var savedDbChanges = await _applicationRepository.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to declinbe Asset request."
                    }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "AssetRequest Declined successfully."
                }
            };
        }
    }
}