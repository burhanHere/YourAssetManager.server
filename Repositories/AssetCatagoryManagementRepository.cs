using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetCatagoryManagementRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
    {
        UserManager<ApplicationUser> _userManager = userManager;
        ApplicationDbContext _applicationDbContext = applicationDbContext;
        public async Task<ApiResponseDTO> GetAllAssetCategories(string signedInUserId)
        {
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
            List<AssetCategory> requiredCategories = await _applicationDbContext.AssetCategories.Where(x => x.CatagoryOrganizationId == organization.Id).ToListAsync();
            if (requiredCategories.Count == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagories found as no organization is associated to this user."
                    }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredCategories
            };
        }
        public async Task<ApiResponseDTO> GetAssetCategoryById(int AssetCatagoryId)
        {
            var requiredAssetCatagory = await _applicationDbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == AssetCatagoryId);
            if (requiredAssetCatagory == null)
            {
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
        public async Task<ApiResponseDTO> CreateAssetCategory(string signedInUserId, AssetCatagoryDTO assetCatagoryDTO)
        {
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

            var organization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == organizationOwner.Id);
            if (organization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "As no organization is allowed to this user so can create catagory"
                    }
                };
            }

            AssetCategory newCatagory = new()
            {
                CategoryName = assetCatagoryDTO.CategoryName.ToUpper(),
                Description = assetCatagoryDTO.Description,
                RelaventInputFields = assetCatagoryDTO.RelaventInputFields,
                CatagoryOrganizationId = organization.Id
            };

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
        public async Task<ApiResponseDTO> UpdateAssetCategory(int AssetCatagoryId, AssetCatagoryDTO assetCatagoryDTO)
        {
            var requiredAssetCatagory = await _applicationDbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == AssetCatagoryId);
            if (requiredAssetCatagory == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagory found with this id."
                    }
                };
            }
            requiredAssetCatagory.CategoryName = assetCatagoryDTO.CategoryName.ToUpper();
            requiredAssetCatagory.Description = assetCatagoryDTO.Description;
            requiredAssetCatagory.RelaventInputFields = assetCatagoryDTO.RelaventInputFields;

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
        public async Task<ApiResponseDTO> DeleteAssetCatagory(int AssetCatagoryId)
        {
            var requiredAssetCatagory = await _applicationDbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == AssetCatagoryId);
            if (requiredAssetCatagory == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No catagory found with this id."
                    }
                };
            }
            var output = await _applicationDbContext.Assets.FirstOrDefaultAsync(x => x.AssetCategoryId == requiredAssetCatagory.Id);
            if (output != null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status403Forbidden,
                    ResponseData = new List<string>
                    {
                        "Cant delete Catagory as it is associated to one or more assets."
                    }
                };
            }
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