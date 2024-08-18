using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetManagementRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<ApiResponseDTO> CreateAsset(string userId, AssetDTO newAssetDTO)
        {
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
            var userOrganization = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(uo => uo.UserId == userId && uo.OrganizationId == newAssetDTO.OrganizationId && uo.Organization.ActiveOrganization);
            if (userOrganization == null)
            {
                // Return error if organization and user association not found or if organization is deleted(deactivated)
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can perform this action as organization is not active or associated to this user.",
                    }
                };
            }

            var statusExist = await _applicationDbContext.AssetStatuses.AnyAsync(s => s.Id == newAssetDTO.AssetStatusId);
            if (!statusExist)
            {
                // Return error if AssetStatus not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Invalid asset status selected.",
                    }
                };
            }

            var catagory = await _applicationDbContext.AssetCategories.AnyAsync(x => x.Id == newAssetDTO.AssetCategoryId && x.CategoryOrganizationId == newAssetDTO.OrganizationId);
            if (!catagory)
            {
                // Return error if catagory not fount or if this catagory is not associated to this the give(input) organizatio id
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid Catagory selected.",
                    }
                };
            }

            var assetType = await _applicationDbContext.AssetTypes.AnyAsync(x => x.Id == newAssetDTO.AssetTypeId && x.OrganizationId == newAssetDTO.OrganizationId);
            if (!assetType)
            {
                // Return error if assetType not fount or if this assetType is not associated to this the give(input) organizatio id
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid assetType request.",
                    }
                };
            }

            var vendor = await _applicationDbContext.Vendors.AnyAsync(v => v.Id == newAssetDTO.VendorId && v.OrganizationId == newAssetDTO.OrganizationId);
            if (!vendor)
            {
                // Return error if vendor not fount or if this vendor is not associated to this the give(input) organizatio id
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid vendor request.",
                        "Vendor not found."
                    }
                };
            }

            Asset newAsset = new()
            {
                AssetName = newAssetDTO.AssetName,
                Description = newAssetDTO.Description,
                PurchaseDate = newAssetDTO.PurchaseDate,
                PurchasePrice = newAssetDTO.PurchasePrice,
                SerialNumber = newAssetDTO.SerialNumber,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                AssetIdentificationNumber = newAssetDTO.AssetIdentificationNumber,
                Manufacturer = newAssetDTO.Manufacturer,
                Model = newAssetDTO.Model,
                AssetStatusId = newAssetDTO.AssetStatusId,
                OrganizationId = newAssetDTO.OrganizationId,
                CatagoryReleventFeildsData = newAssetDTO.CatagoryReleventFeildsData,
                AssetCategoryId = newAssetDTO.AssetCategoryId,
                AssetTypeId = newAssetDTO.AssetTypeId,
                VendorId = newAssetDTO.VendorId,
            };

            await _applicationDbContext.Assets.AddAsync(newAsset);


            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to create new Asset."
                    }
                };
            }

            // Return success if the new asset was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "New Asset created successfully."
                    }
            };
        }

    }
}