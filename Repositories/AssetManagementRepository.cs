using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetManagementRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        public async Task<ApiResponseDTO> GetAssetManagers(string OrganizationOwnerUserName)
        {
            var organizationOwner = await _userManager.FindByNameAsync(OrganizationOwnerUserName);
            if (organizationOwner == null)
            {
                // Return error if user not found
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
            // var organization = await _applicationDbContext;
            var assetManagers = await _userManager.GetUsersInRoleAsync("AssetManager");
            if (assetManagers == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "No asset managers found." }
                };
            }
            var requiredAssetManager = assetManagers.Where(a => a.OrganizationId == organizationOwner.OrganizationId).Select(a => a);
            if (requiredAssetManager == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "No asset managers found related to this organizations.", }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new
                {
                    count = requiredAssetManager.Count(),
                    assetManagers = requiredAssetManager
                }
            };
        }

        public async Task<ApiResponseDTO> CreateNewUser(string OrganizationOwnerUserName, NewUserDTO employeeDTO)
        {
            var organizationOwner = await _userManager.FindByNameAsync(OrganizationOwnerUserName);
            if (organizationOwner == null)
            {
                // Return error if user not found
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
            return new ApiResponseDTO();
        }

        public async Task<ApiResponseDTO> GetAssetCategories(string OrganizationOwnerUserName)
        {
            var organizationOwner = await _userManager.FindByNameAsync(OrganizationOwnerUserName);
            if (organizationOwner == null)
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
            var organization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ActiveOrganization == true && x.ApplicationUserId == organizationOwner.Id);
            if (organization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagories found as noorganization is associated to this user."
                    }
                };
            }
            var requiredCategories = await _applicationDbContext.AssetCategories.Where(x => x.CatagoryOrganizationId == organization.Id).ToListAsync();
            if (requiredCategories.Count == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagories found as noorganization is associated to this user."
                    }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredCategories
            };
        }
    }
}