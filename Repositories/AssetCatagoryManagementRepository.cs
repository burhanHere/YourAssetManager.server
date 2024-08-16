using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    /// <summary>
    /// Repository for managing asset categories and subcategories.
    /// </summary>
    public class AssetCatagoryManagementRepository(UserManager<IdentityUser> userManager, ApplicationDbContext applicationDbContext)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetCategoryManagementRepository"/> class.
        /// </summary>
        /// <param name="userManager">The user manager to handle user-related operations.</param>
        /// <param name="applicationDbContext">The database context for accessing application data.</param>
        UserManager<IdentityUser> _userManager = userManager;
        ApplicationDbContext _applicationDbContext = applicationDbContext;
        /// <summary>
        /// Retrieves all asset categories associated with the signed-in user.
        /// </summary>
        /// <param name="signedInUserId">The ID of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAllAssetCategories(string signedInUserId)
        {
            // Find the organization owner by user ID
            var user = await _userManager.FindByIdAsync(signedInUserId);
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

            // Retrieve all categories associated with the organization
            List<AssetCategory> requiredCategories = await _applicationDbContext.AssetCategories.Where(x => x.CategoryOrganizationId == userOrganization.OrganizationId).Select(x => x).ToListAsync();
            if (requiredCategories.Count == 0)
            {
                // Return error if no categories found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagories found in this organization of this user."
                    }
                };
            }

            List<AssetCatagoryDTO> assetCatagoryDTOList = new();
            foreach (var item in requiredCategories)
            {
                assetCatagoryDTOList.Add(new AssetCatagoryDTO
                {
                    Id = item.Id,
                    CategoryName = item.CategoryName,
                    Description = item.Description,
                    RelaventInputFields = item.RelevantInputFields,
                });
            }
            // Return success with the list of categories
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = assetCatagoryDTOList
            };
        }

        /// <summary>
        /// Retrieves an asset category by its ID.
        /// </summary>
        /// <param name="assetCategoryId">The ID of the asset category.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAssetCategoryById(int AssetCatagoryId)
        {
            // Find the category by ID
            var requiredAssetCatagory = await _applicationDbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == AssetCatagoryId);
            if (requiredAssetCatagory == null)
            {
                // Return error if category not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagory found with this id."
                    }
                };
            }

            AssetCatagoryDTO assetCatagoryDTO = new()
            {
                Id = requiredAssetCatagory.Id,
                CategoryName = requiredAssetCatagory.CategoryName,
                Description = requiredAssetCatagory.Description,
                RelaventInputFields = requiredAssetCatagory.RelevantInputFields,
            };

            // Return success if the catagory was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = assetCatagoryDTO
            };
        }

        /// <summary>
        /// Creates a new asset category for the signed-in user.
        /// </summary>
        /// <param name="signedInUserId">The ID of the signed-in user.</param>
        /// <param name="assetCategoryDTO">The data transfer object containing category details.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> CreateAssetCategory(string signedInUserId, AssetCatagoryDTO assetCatagoryDTO)
        {
            // Find the organization owner by user ID
            var user = await _userManager.FindByIdAsync(signedInUserId);
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

            // Create a new asset category
            AssetCategory newCatagory = new()
            {
                CategoryName = assetCatagoryDTO.CategoryName.ToUpper(),
                Description = assetCatagoryDTO.Description,
                RelevantInputFields = assetCatagoryDTO.RelaventInputFields,
                CategoryOrganizationId = userOrganization.OrganizationId
            };

            // Add the new category to the database
            await _applicationDbContext.AssetCategories.AddAsync(newCatagory);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();

            if (savedDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to create new Catagory."
                    }
                };
            }

            // Return success if the catagory was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "New Catagory created successfully."
                    }
            };
        }

        /// <summary>
        /// Updates an existing asset category.
        /// </summary>
        /// <param name="assetCategoryId">The ID of the asset category to update.</param>
        /// <param name="assetCategoryDTO">The data transfer object containing updated category details.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> UpdateAssetCategory(AssetCatagoryDTO assetCatagoryDTO)
        {
            // Find the category by ID
            var requiredAssetCatagory = await _applicationDbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == assetCatagoryDTO.Id);
            if (requiredAssetCatagory == null)
            {
                // Return error if category not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagory found with this id."
                    }
                };
            }

            // Update category properties with new values
            requiredAssetCatagory.CategoryName = assetCatagoryDTO.CategoryName.IsNullOrEmpty() ? requiredAssetCatagory.CategoryName : assetCatagoryDTO.CategoryName.ToUpper();

            requiredAssetCatagory.Description = assetCatagoryDTO.Description.IsNullOrEmpty() ? requiredAssetCatagory.Description : assetCatagoryDTO.Description;

            requiredAssetCatagory.RelevantInputFields = assetCatagoryDTO.RelaventInputFields.IsNullOrEmpty() ? requiredAssetCatagory.RelevantInputFields : assetCatagoryDTO.RelaventInputFields;

            // Save changes to the database
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to update Catagory."
                    }
                };
            }

            // Return success if the catagory was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "New Catagory updated successfully."
                    }
            };
        }

        /// <summary>
        /// Deletes an existing asset category.
        /// </summary>
        /// <param name="assetCategoryId">The ID of the asset category to delete.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> DeleteAssetCatagory(int AssetCatagoryId)
        {
            // Find the category by ID
            var requiredAssetCatagory = await _applicationDbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == AssetCatagoryId);
            if (requiredAssetCatagory == null)
            {
                // Return error if category not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagory found with this id."
                    }
                };
            }

            // Check if the category is associated with any assets
            var associatedAssets = await _applicationDbContext.Assets
               .AnyAsync(x => x.AssetCategoryId == AssetCatagoryId);
            if (associatedAssets)
            {
                // Return error if category is associated with assets
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Cannot delete category as it is associated with one or more assets."
                    }
                };
            }

            // Remove the category from the database
            _applicationDbContext.AssetCategories.Remove(requiredAssetCatagory);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to delete Catagory."
                    }
                };
            }

            // Return success if the catagory was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Catagory deleted successfully."
                    }
            };
        }
    }
}