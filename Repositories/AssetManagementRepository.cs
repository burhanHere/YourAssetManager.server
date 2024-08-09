using Microsoft.AspNetCore.Identity;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetManagementRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        public async Task<ApiResponceDTO> GetAssetManagers(string OrganizationOwnerUserName)
        {
            var organizationOwner = await _userManager.FindByNameAsync(OrganizationOwnerUserName);
            if (organizationOwner == null)
            {
                // Return error if user not found
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            // var organization = await _applicationDbContext;
            var assetManagers = await _userManager.GetUsersInRoleAsync("AssetManager");
            if (assetManagers == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string> { "No asset managers found." }
                };
            }
            var requiredAssetManager = assetManagers.Where(a => a.OrganizationId == organizationOwner.OrganizationId).Select(a => a);
            if (requiredAssetManager == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string> { "No asset managers found related to this organizations.", }
                };
            }
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new
                {
                    count = requiredAssetManager.Count(),
                    assetManagers = requiredAssetManager
                }
            };
        }

        public async Task<ApiResponceDTO> CreateNewUser(string OrganizationOwnerUserName, NewUserDTO employeeDTO)
        {
            var organizationOwner = await _userManager.FindByNameAsync(OrganizationOwnerUserName);
            if (organizationOwner == null)
            {
                // Return error if user not found
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            return new ApiResponceDTO();
        }

        public async Task<ApiResponceDTO> GetAssetCategories(string OrganizationOwnerUserName)
        {
            var organizationOwner = await _userManager.FindByNameAsync(OrganizationOwnerUserName);
            if (organizationOwner == null)
            {
                // Return error if user not found
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            return new ApiResponceDTO();
        }
    }
}