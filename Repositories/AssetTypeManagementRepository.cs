using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    /// <summary>
    /// Repository for handling asset type-related tasks.
    /// </summary>
    public class AssetTypeManagementRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
    {
        // Field for database context
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        /// <summary>
        /// Creates a new asset type for the signed-in user's organization.
        /// </summary>
        /// <param name="userId">The ID of the signed-in user.</param>
        /// <param name="assetType">The new asset type's data.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> CreateAssetType(string userId, AssetTypeDTO assetType)
        {
            // Find the organization associated with the user
            var user = await _userManager.FindByIdAsync(userId);
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
            // Create the new asset type entity
            AssetType newAssetType = new()
            {
                AssetTypeName = assetType.AssetTypeName,
                Description = assetType.Description,
                OrganizationId = userOrganization.OrganizationId,
            };

            // Add the new asset type to the database
            await _applicationDbContext.AssetTypes.AddAsync(newAssetType);

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string> { "Asset type creation failed." }
                };
            }

            // Return success if the asset type was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string> { "Asset type created successfully." }
            };
        }

        /// <summary>
        /// Retrieves all asset types associated with the signed-in user's organization.
        /// </summary>
        /// <param name="userId">The ID of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAllAssetTypes(string userId)
        {
            // Find the organization associated with the user
            var user = await _userManager.FindByIdAsync(userId);
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
            // Query the asset types associated with the organization
            var assetTypes = await _applicationDbContext.AssetTypes.Where(x => x.OrganizationId == userOrganization.OrganizationId).ToListAsync();
            if (assetTypes.Count == 0)
            {
                // Return success but indicate no asset types found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "No asset types found." }
                };
            }

            // Convert to DTO
            List<AssetTypeDTO> assetTypeDTOList = new();
            foreach (var item in assetTypes)
            {
                assetTypeDTOList.Add(new AssetTypeDTO
                {
                    Id = item.Id,
                    AssetTypeName = item.AssetTypeName,
                    Description = item.Description
                });
            }

            // Return the list of asset types
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = assetTypeDTOList
            };
        }

        /// <summary>
        /// Retrieves an asset type by its ID.
        /// </summary>
        /// <param name="assetTypeId">The ID of the asset type.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAssetTypeById(string userId, int assetTypeId)
        {
            // Find the organization associated with the user
            var user = await _userManager.FindByIdAsync(userId);
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
            // Find the asset type by its ID
            var assetType = await _applicationDbContext.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetTypeId && x.OrganizationId == userOrganization.OrganizationId);

            // Check if the asset type exists
            if (assetType == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "Asset type not found." }
                };
            }

            // Convert to DTO
            var assetTypeDTO = new AssetTypeDTO
            {
                Id = assetType.Id,
                AssetTypeName = assetType.AssetTypeName,
                Description = assetType.Description
            };

            // Return the asset type DTO
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = assetTypeDTO
            };
        }

        /// <summary>
        /// Updates the details of an existing asset type.
        /// </summary>
        /// <param name="assetTypeUpdate">The asset type's updated data.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> UpdateAssetType(string userId, AssetTypeDTO assetTypeUpdate)
        {
            // Find the organization associated with the user
            var user = await _userManager.FindByIdAsync(userId);
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

            // Find the asset type by its ID
            var existingAssetType = await _applicationDbContext.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetTypeUpdate.Id && x.OrganizationId == userOrganization.OrganizationId);

            // Check if the asset type exists
            if (existingAssetType == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "Asset type not found." }
                };
            }

            // Update asset type properties with new values if provided
            existingAssetType.AssetTypeName = assetTypeUpdate.AssetTypeName.IsNullOrEmpty() ? existingAssetType.AssetTypeName : assetTypeUpdate.AssetTypeName;
            existingAssetType.Description = assetTypeUpdate.Description.IsNullOrEmpty() ? existingAssetType.Description : assetTypeUpdate.Description;

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string> { "Asset type update failed." }
                };
            }

            // Return success if the asset type was updated successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string> { "Asset type updated successfully." }
            };
        }

        /// <summary>
        /// Deletes an asset type by its ID.
        /// </summary>
        /// <param name="assetTypeId">The ID of the asset type to delete.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> DeleteAssetType(string userId, int assetTypeId)
        {
            // Find the organization associated with the user
            var user = await _userManager.FindByIdAsync(userId);
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

            // Find the asset type by its ID
            var assetType = await _applicationDbContext.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetTypeId && x.OrganizationId == userOrganization.OrganizationId);
            if (assetType == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "Asset type not found." }
                };
            }

            // Check if the asset type is associated with any assets
            var associatedAsset = await _applicationDbContext.Assets.AnyAsync(x => x.AssetTypeId == assetType.Id);
            if (associatedAsset)
            {
                // Return error if the asset type is associated with assets
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Cannot delete asset type as it is associated with one or more assets."
                    }
                };
            }

            // Remove the asset type from the database
            _applicationDbContext.AssetTypes.Remove(assetType);

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string> { "Asset type deletion failed." }
                };
            }

            // Return success if the asset type was deleted successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string> { "Asset type deleted successfully." }
            };
        }
    }
}
