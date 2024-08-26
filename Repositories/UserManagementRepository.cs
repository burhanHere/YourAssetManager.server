using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Services;

namespace YourAssetManager.Server.Repositories
{
    public class UserManagementRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext, IConfiguration configuration)
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly CloudinaryService _cloudinaryService = new(configuration);
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

        public async Task<ApiResponseDTO> GetAllUsers(string currectLogedInUserId)
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

            HashSet<UserDTO> requireduserAccounts = _applicationDbContext.UserOrganizations.Where(x => x.OrganizationId == userOrganization.OrganizationId)
            .Select(x => new UserDTO
            {
                Id = x.User.Id,
                UserName = x.User.UserName,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber,
                ActiveUser = x.User.ActiveUser,
                ImagePath = x.User.ImagePath
            }).ToHashSet();

            var userIds = requireduserAccounts.Select(x => x.Id).ToList();

            var userRoles = await _applicationDbContext.UserRoles.Where(x => userIds.Contains(x.UserId)).ToListAsync();

            var Roles = await _applicationDbContext.Roles.ToDictionaryAsync(x => x.Id, x => x.Name);

            foreach (var item in userRoles)
            {
                var roleName = Roles[item.RoleId];
                if (!roleName.IsNullOrEmpty())
                {
                    var temp = requireduserAccounts.First(x => x.Id == item.UserId);
                    if (temp.Roles == null)
                    {
                        temp.Roles = new List<string>
                        {
                            roleName
                        };
                    }
                    else
                    {
                        temp.Roles.Add(roleName);
                    }
                }
            }
            Console.WriteLine(requireduserAccounts);
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requireduserAccounts
            };
        }
        public async Task<ApiResponseDTO> GetUserById(string currectLogedInUserId, string targetUserId)
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
                    ResponseData = new List<string> { "Target User not found." }
                };
            }
            var Roles = await _applicationDbContext.Roles.ToDictionaryAsync(x => x.Id, x => x.Name);

            var userRole = await _userManager.GetRolesAsync(targetUser);

            UserDTO reaponseData = new()
            {
                Id = targetUser.Id,
                UserName = targetUser.UserName,
                Email = targetUser.Email,
                PhoneNumber = targetUser.PhoneNumber,
                ActiveUser = targetUser.ActiveUser,
                Roles = userRole.ToList(),
                ImagePath = targetUser.ImagePath,
            };
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = reaponseData
            };
        }
        public async Task<ApiResponseDTO> UpdateUserProfile(string targetUserId, UserProfileUpdateDTO userProfileUpdateDTO)
        {
            ApplicationUser targetUser = await _userManager.FindByIdAsync(targetUserId);
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
            //check us userName is unique or not
            bool duplicateUserName = await _applicationDbContext.Users.AnyAsync(x => x.UserName == userProfileUpdateDTO.UserName);
            if (duplicateUserName)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status409Conflict,
                    ResponseData = new List<string>
                    {
                        "User name already in use by some other user.",
                        "User name must be unique."
                    }
                };
            }
            var stream = userProfileUpdateDTO.ProfilePicture.OpenReadStream();
            string cloudinaryUrlOfImage = await _cloudinaryService.UploadImageToCloudinaryAsync(stream, userProfileUpdateDTO.ProfilePicture.FileName);
            if (string.IsNullOrEmpty(cloudinaryUrlOfImage))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>()
                    {
                        "failed to update profile."
                    }
                };
            }

            targetUser.UserName = userProfileUpdateDTO.UserName.IsNullOrEmpty()?targetUser.UserName:userProfileUpdateDTO.UserName;
            targetUser.ImagePath = cloudinaryUrlOfImage;

            _applicationDbContext.Users.Update(targetUser);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>()
                    {
                        "failed to update profile."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>()
                    {
                        "Profile updated Successfully."
                    }
            };
        }
    }
}