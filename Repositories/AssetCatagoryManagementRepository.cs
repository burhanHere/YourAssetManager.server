using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    /// <summary>
    /// Repository for managing asset categories and subcategories.
    /// </summary>
    public class AssetCatagoryManagementRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetCategoryManagementRepository"/> class.
        /// </summary>
        /// <param name="userManager">The user manager to handle user-related operations.</param>
        /// <param name="applicationDbContext">The database context for accessing application data.</param>
        UserManager<ApplicationUser> _userManager = userManager;
        ApplicationDbContext _applicationDbContext = applicationDbContext;
        /// <summary>
        /// Retrieves all asset categories associated with the signed-in user.
        /// </summary>
        /// <param name="signedInUserId">The ID of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAllAssetCategories(string signedInUserId)
        {
            // Find the organization owner by user ID
            var organizationOwner = await _userManager.FindByIdAsync(signedInUserId);
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

            // Find the active organization associated with the user
            var organization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ActiveOrganization == true && x.ApplicationUserId == organizationOwner.Id);
            if (organization == null)
            {
                // Return error if no organization is associated with the user
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagories found as noorganization is associated to this user."
                    }
                };
            }

            // Retrieve all categories associated with the organization
            List<AssetCategory> requiredCategories = await _applicationDbContext.AssetCategories.Where(x => x.CatagoryOrganizationId == organization.Id).Select(x => x).ToListAsync();
            if (requiredCategories.Count == 0)
            {
                // Return error if no categories found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagories found as no organization is associated to this user."
                    }
                };
            }

            // Return success with the list of categories
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredCategories
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
        /// Creates a new asset category for the signed-in user.
        /// </summary>
        /// <param name="signedInUserId">The ID of the signed-in user.</param>
        /// <param name="assetCategoryDTO">The data transfer object containing category details.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> CreateAssetCategory(string signedInUserId, AssetCatagoryDTO assetCatagoryDTO)
        {
            // Find the organization owner by user ID
            var organizationOwner = await _userManager.FindByIdAsync(signedInUserId);
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

            // Find the organization associated with the user
            var organization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == organizationOwner.Id);
            if (organization == null)
            {
                // Return error if no organization is associated with the user
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "As no organization is associated to this user so can create catagory"
                    }
                };
            }

            // Create a new asset category
            AssetCategory newCatagory = new()
            {
                CategoryName = assetCatagoryDTO.CategoryName.ToUpper(),
                Description = assetCatagoryDTO.Description,
                RelaventInputFields = assetCatagoryDTO.RelaventInputFields,
                CatagoryOrganizationId = organization.Id
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
        public async Task<ApiResponseDTO> UpdateAssetCategory(int AssetCatagoryId, AssetCatagoryDTO assetCatagoryDTO)
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

            // Update category properties with new values
            requiredAssetCatagory.CategoryName = assetCatagoryDTO.CategoryName.ToUpper();
            requiredAssetCatagory.Description = assetCatagoryDTO.Description;
            requiredAssetCatagory.RelaventInputFields = assetCatagoryDTO.RelaventInputFields;

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

        /// <summary>
        /// Retrieves all asset subcategories associated with a specific asset category ID.
        /// </summary>
        /// <param name="assetCategoryId">The ID of the asset category.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAllAssetSubCategoriesByAssetCaragoryId(int assetCategoryId)
        {
            // Find the asset category by ID
            var assetCategory = await _applicationDbContext.AssetCategories
                .FirstOrDefaultAsync(x => x.Id == assetCategoryId);
            if (assetCategory == null)
            {
                // Return error if category not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No category found with this id."
                    }
                };
            }

            // Retrieve all subcategories associated with the category
            List<AssetSubCategory> subCategories = await _applicationDbContext.AssetSubCategories
                .Where(x => x.AssetCategoryId == assetCategoryId)
                .ToListAsync();
            if (subCategories.Count == 0)
            {
                // Return error if no subcategories found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No subcategories found for this category."
                    }
                };
            }

            // Return success with the list of subcategories
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = subCategories
            };
        }

        /// <summary>
        /// Retrieves an asset subcategory by its ID.
        /// </summary>
        /// <param name="assetSubCategoryId">The ID of the asset subcategory.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAssetSubCategoryById(int assetSubCategoryId)
        {
            // Find the subcategory by ID
            var subCategory = await _applicationDbContext.AssetSubCategories
                .FirstOrDefaultAsync(x => x.Id == assetSubCategoryId);
            if (subCategory == null)
            {
                // Return error if subcategory not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No subcategory found with this id."
                    }
                };
            }

            // Return success with the subcategory details
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = subCategory
            };
        }

        /// <summary>
        /// Creates a new asset subcategory for a specific asset category.
        /// </summary>
        /// <param name="assetCategoryId">The ID of the asset category.</param>
        /// <param name="assetSubCatagoryDTO">The data transfer object containing subcategory details.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> CreateAssetSubCategory(int assetCategoryId, AssetSubCatagoryDTO assetSubCatagoryDTO)
        {
            // Find the asset category by ID
            var assetCategory = await _applicationDbContext.AssetCategories
                .FirstOrDefaultAsync(x => x.Id == assetCategoryId);
            if (assetCategory == null)
            {
                // Return error if category not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No category found with this id."
                    }
                };
            }

            // Create a new asset subcategory
            AssetSubCategory newSubCategory = new()
            {
                SubCategoryName = assetSubCatagoryDTO.SubCategoryName.ToUpper(),
                AssetCategoryId = assetCategoryId
            };

            // Add the new subcategory to the database
            await _applicationDbContext.AssetSubCategories.AddAsync(newSubCategory);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();

            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    // Return error if saving to the database failed
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to create new subcategory."
                    }
                };
            }

            // Return success if the subcategory was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "New subcategory created successfully."
                }
            };
        }

        /// <summary>
        /// Updates an existing asset subcategory.
        /// </summary>
        /// <param name="assetSubCategoryId">The ID of the asset subcategory to update.</param>
        /// <param name="assetSubCatagoryDTO">The data transfer object containing updated subcategory details.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> UpdateAssetSubCategory(int assetSubCategoryId, AssetSubCatagoryDTO assetSubCatagoryDTO)
        {
            // Find the subcategory by ID
            var subCategory = await _applicationDbContext.AssetSubCategories
                .FirstOrDefaultAsync(x => x.Id == assetSubCategoryId);
            if (subCategory == null)
            {
                // Return error if subcategory not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No subcategory found with this id."
                    }
                };
            }

            // Update subcategory properties with new values
            subCategory.SubCategoryName = assetSubCatagoryDTO.SubCategoryName.ToUpper();

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
                        "Unable to update subcategory."
                    }
                };
            }

            // Return success if the subcategory was updated successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Subcategory updated successfully."
                }
            };
        }

        /// <summary>
        /// Deletes an existing asset subcategory.
        /// </summary>
        /// <param name="assetSubCategoryId">The ID of the asset subcategory to delete.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> DeleteAssetSubCategory(int assetSubCategoryId)
        {
            // Find the subcategory by ID
            var subCategory = await _applicationDbContext.AssetSubCategories
                .FirstOrDefaultAsync(x => x.Id == assetSubCategoryId);
            if (subCategory == null)
            {
                // Return error if subcategory not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No subcategory found with this id."
                    }
                };
            }

            // Check if the subcategory is associated with any assets
            var associatedAssets = await _applicationDbContext.Assets
                .AnyAsync(x => x.AssetSubCategoryId == assetSubCategoryId);
            if (associatedAssets)
            {
                // Return error if subcategory is associated with assets
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status403Forbidden,
                    ResponseData = new List<string>
                    {
                        "Cannot delete subcategory as it is associated with one or more assets."
                    }
                };
            }

            // Remove the subcategory from the database
            _applicationDbContext.AssetSubCategories.Remove(subCategory);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();

            if (savedDbChanges == 0)
            {
                // Return error if deletion failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to delete subcategory."
                    }
                };
            }

            // Return success if the subcategory was deleted successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Subcategory deleted successfully."
                }
            };
        }
    }
}