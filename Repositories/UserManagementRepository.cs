using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class UserManagementRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
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

        public async Task<ApiResponseDTO> DeactivateAccount(string currectLogedInUserId, string targetUserId)
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

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
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

            var isOrganizationOwner = await _userManager.IsInRoleAsync(targetUser, "OrganizationOwner");
            if (isOrganizationOwner)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't perform this actions on organization owner."
                    }
                };
            }

            targetUser.ActiveUser = false;
            var result = await _userManager.UpdateAsync(targetUser);
            if (!result.Succeeded)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to Deactivate target user."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Target user Deactivated successfully."
                    }
            };
        }

        public async Task<ApiResponseDTO> ActivateAccount(string currectLogedInUserId, string targetUserId)
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

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
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

            var isOrganizationOwner = await _userManager.IsInRoleAsync(targetUser, "OrganizationOwner");
            if (isOrganizationOwner)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't perform this actions on organization owner."
                    }
                };
            }

            targetUser.ActiveUser = true;
            var result = await _userManager.UpdateAsync(targetUser);
            if (!result.Succeeded)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to Activate target user."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Target user Activated successfully."
                    }
            };
        }
    }
}