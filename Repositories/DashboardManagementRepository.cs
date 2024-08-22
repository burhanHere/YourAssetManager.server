using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;

namespace YourAssetManager.Server.Models
{
    public class DashboardManagementRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        public async Task<ApiResponseDTO> GetDashBoardStatiticsData(string signedInUserId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(signedInUserId);
            if (user == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }

            // Check if the user already has an active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);
            if (userOrganization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No active organization found for the user."
                    }
                };
            }

            int vendorCount = await _applicationDbContext.Vendors.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId);
            int CatagoryCount = await _applicationDbContext.AssetCategories.CountAsync(x => x.CategoryOrganizationId == userOrganization.OrganizationId);
            int AssetTypeCount = await _applicationDbContext.AssetTypes.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId);
            int AssetCount = await _applicationDbContext.Assets.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId);
            int AssignedAssetCount = await _applicationDbContext.Assets.CountAsync(x => x.AssetStatusId == 1 && x.OrganizationId == userOrganization.OrganizationId);
            int RetiredAssetCount = await _applicationDbContext.Assets.CountAsync(x => x.AssetStatusId == 2 && x.OrganizationId == userOrganization.OrganizationId);
            int UnderMaintenanceAssetCount = await _applicationDbContext.Assets.CountAsync(x => x.AssetStatusId == 3 && x.OrganizationId == userOrganization.OrganizationId);
            int AvailableAssetCount = await _applicationDbContext.Assets.CountAsync(x => x.AssetStatusId == 5 && x.OrganizationId == userOrganization.OrganizationId);
            int userCount = await _applicationDbContext.UserOrganizations.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId && x.Organization.ActiveOrganization == true);
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new
                {
                    vendorCount = vendorCount,
                    CatagoryCount = CatagoryCount,
                    AssetTypeCount = AssetTypeCount,
                    userCount = userCount,
                    Asset = new
                    {
                        AssetCount = AssetCount,
                        AssignedAssetCount = AssignedAssetCount,
                        RetiredAssetCount = RetiredAssetCount,
                        UnderMaintenanceAssetCount = UnderMaintenanceAssetCount,
                        AvailableAssetCount = AvailableAssetCount,
                    }
                }
            };
        }

    }
}