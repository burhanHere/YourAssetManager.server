using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;

namespace YourAssetManager.Server.Repositories
{
    public class UserManagementRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        public async Task<ApiResponseDTO> AppointAssetManager(string currectLogedInUserId, string AppointeeId)
        {
            // Find the organization owner by user ID
            var user = await _userManager.FindByIdAsync(currectLogedInUserId);
            if (user == null)
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
            // Find the user's active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
               .FirstOrDefaultAsync(uo => uo.UserId == user.Id && uo.Organization.ActiveOrganization);

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

            var appointeeUser = await _userManager.FindByIdAsync(AppointeeId);
            if (appointeeUser == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Target user not fount.",
                    }
                };
            }

            // assigning Employee role to the registerer
            var roleName = "AssetManager";
            var employeeRole = await _roleManager.FindByNameAsync(roleName);
            if (employeeRole == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status500InternalServerError,
                    ResponseData = new List<string>
                        {
                            $"Role '{roleName}' does not exist. Please contact support."
                        }
                };
            }

            var addToUserRoleResult = await _userManager.AddToRoleAsync(appointeeUser, employeeRole.Name!);
            if (!addToUserRoleResult.Succeeded)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to Appointed as AssetManager."
                    }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Appointed as AssetManager."
                }
            };
        }

        public async Task<ApiResponseDTO> DismissAssetManager(string currectLogedInUserId, string AppointeeId)
        {
            // Find the organization owner by user ID
            var user = await _userManager.FindByIdAsync(currectLogedInUserId);
            if (user == null)
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
            // Find the user's active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
               .FirstOrDefaultAsync(uo => uo.UserId == user.Id && uo.Organization.ActiveOrganization);

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

            var appointeeUser = await _userManager.FindByIdAsync(AppointeeId);
            if (appointeeUser == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Target user not fount.",
                    }
                };
            }

            // assigning Employee role to the registerer
            var roleName = "AssetManager";
            var employeeRole = await _roleManager.FindByNameAsync(roleName);
            if (employeeRole == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status500InternalServerError,
                    ResponseData = new List<string>
                        {
                            $"Role '{roleName}' does not exist. Please contact support."
                        }
                };
            }

            var addToUserRoleResult = await _userManager.RemoveFromRoleAsync(appointeeUser, employeeRole.Name!);
            if (!addToUserRoleResult.Succeeded)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to Dismiss from AssetManager Role."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Dismissed from AssetManager Role"
                }
            };
        }
    }
}