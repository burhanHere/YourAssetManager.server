using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            var userOrganization = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(uo => uo.UserId == userId && uo.OrganizationId == int.Parse(newAssetDTO.OrganizationData) && uo.Organization.ActiveOrganization);
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

            var catagory = await _applicationDbContext.AssetCategories.AnyAsync(x => x.Id == int.Parse(newAssetDTO.AssetCategoryData) && x.CategoryOrganizationId == int.Parse(newAssetDTO.OrganizationData));
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

            var assetType = await _applicationDbContext.AssetTypes.AnyAsync(x => x.Id == int.Parse(newAssetDTO.AssetTypeData) && x.OrganizationId == int.Parse(newAssetDTO.OrganizationData));
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

            var vendor = await _applicationDbContext.Vendors.AnyAsync(v => v.Id == int.Parse(newAssetDTO.VendorData) && v.OrganizationId == int.Parse(newAssetDTO.OrganizationData));
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
                CatagoryReleventFeildsData = newAssetDTO.CatagoryReleventFeildsData,
                AssetStatusId = 5,// 5 is the id of available; byu default asset will be available
                OrganizationId = int.Parse(newAssetDTO.OrganizationData),
                AssetCategoryId = int.Parse(newAssetDTO.AssetCategoryData),
                AssetTypeId = int.Parse(newAssetDTO.AssetTypeData),
                VendorId = int.Parse(newAssetDTO.VendorData),
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

        public async Task<ApiResponseDTO> UpdateAsset(string userId, AssetDTO assetDtoUpdate)
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

            var targetAsset = await _applicationDbContext.Assets.FirstOrDefaultAsync(x => x.AssetId == assetDtoUpdate.Id);
            if (targetAsset == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Asset Updation Failed.",
                        "Target Asset is not found."
                    }
                };
            }

            targetAsset.AssetName = assetDtoUpdate.AssetName.IsNullOrEmpty() ? targetAsset.AssetName : assetDtoUpdate.AssetName;

            targetAsset.Description = assetDtoUpdate.Description.IsNullOrEmpty() ? targetAsset.Description : assetDtoUpdate.Description;

            targetAsset.PurchaseDate = assetDtoUpdate.PurchaseDate == DateTime.MinValue ? targetAsset.PurchaseDate : assetDtoUpdate.PurchaseDate;

            targetAsset.PurchasePrice = assetDtoUpdate.PurchasePrice == 0 ? targetAsset.PurchasePrice : assetDtoUpdate.PurchasePrice;

            targetAsset.SerialNumber = assetDtoUpdate.SerialNumber.IsNullOrEmpty() ? targetAsset.SerialNumber : assetDtoUpdate.SerialNumber;

            targetAsset.UpdatedDate = DateTime.Now;

            targetAsset.AssetIdentificationNumber = assetDtoUpdate.AssetIdentificationNumber.IsNullOrEmpty() ? null : assetDtoUpdate.AssetIdentificationNumber;

            targetAsset.Manufacturer = assetDtoUpdate.Manufacturer.IsNullOrEmpty() ? targetAsset.Manufacturer : assetDtoUpdate.Manufacturer;

            targetAsset.Model = assetDtoUpdate.Model.IsNullOrEmpty() ? targetAsset.Model : assetDtoUpdate.Model;

            targetAsset.CatagoryReleventFeildsData = assetDtoUpdate.CatagoryReleventFeildsData.IsNullOrEmpty() ? null : assetDtoUpdate.CatagoryReleventFeildsData;

            targetAsset.AssetStatusId = int.Parse(assetDtoUpdate.AssetStatusData) == 0 ? targetAsset.AssetStatusId : int.Parse(assetDtoUpdate.AssetStatusData);

            targetAsset.AssetCategoryId = int.Parse(assetDtoUpdate.AssetCategoryData) == 0 ? targetAsset.AssetCategoryId : int.Parse(assetDtoUpdate.AssetCategoryData);

            targetAsset.AssetTypeId = int.Parse(assetDtoUpdate.AssetTypeData) == 0 ? targetAsset.AssetTypeId : int.Parse(assetDtoUpdate.AssetTypeData);

            targetAsset.VendorId = int.Parse(assetDtoUpdate.VendorData) == 0 ? targetAsset.VendorId : int.Parse(assetDtoUpdate.VendorData);

            _applicationDbContext.Assets.Update(targetAsset);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Asset Updation failed.",
                        "Try Again later."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Asset updated successfully.",
                    }
            };
        }

        public async Task<ApiResponseDTO> GetAllAssets(string userId)
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

            var userOrganization = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization);
            if (userOrganization == null)
            {
                // Return error if user not associated to any organization
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not associated to any organization."
                    }
                };
            }

            List<AssetDTO> requiredAssets = await _applicationDbContext.Assets
                .Where(x => x.OrganizationId == userOrganization.OrganizationId)
                .Select(x => new AssetDTO
                {
                    Id = x.AssetId,
                    AssetName = x.AssetName,
                    Description = x.Description,
                    PurchaseDate = x.PurchaseDate,
                    PurchasePrice = x.PurchasePrice,
                    SerialNumber = x.SerialNumber,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    AssetIdentificationNumber = x.AssetIdentificationNumber,
                    Manufacturer = x.Manufacturer,
                    Model = x.Model,
                    CatagoryReleventFeildsData = x.CatagoryReleventFeildsData,
                    AssetStatusData = x.AssetStatus.StatusName,
                    OrganizationData = x.Organization.OrganizationName,
                    AssetCategoryData = x.AssetCategory.CategoryName,
                    AssetTypeData = x.AssetType.AssetTypeName,
                    VendorData = x.Vendor.Name,
                }).ToListAsync();

            if (requiredAssets.Count == 0)
            {
                // Return error if no assets associated to this organization
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No assets found",
                    }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredAssets
            };
        }

        public async Task<ApiResponseDTO> GetAssetsById(string userId, int AssetId)
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

            var userOrganization = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization);
            if (userOrganization == null)
            {
                // Return error if user not associated to any organization
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not associated to any organization."
                    }
                };
            }

            var requiredAssetDTO = await _applicationDbContext.Assets
            .Where(x => x.AssetId == AssetId && x.OrganizationId == userOrganization.OrganizationId)
            .Select(x => new AssetDTO
            {
                Id = x.AssetId,
                AssetName = x.AssetName,
                Description = x.Description,
                PurchaseDate = x.PurchaseDate,
                PurchasePrice = x.PurchasePrice,
                SerialNumber = x.SerialNumber,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                AssetIdentificationNumber = x.AssetIdentificationNumber,
                Manufacturer = x.Manufacturer,
                Model = x.Model,
                CatagoryReleventFeildsData = x.CatagoryReleventFeildsData,
                AssetStatusData = x.AssetStatus.StatusName,
                OrganizationData = x.Organization.OrganizationName,
                AssetCategoryData = x.AssetCategory.CategoryName,
                AssetTypeData = x.AssetType.AssetTypeName,
                VendorData = x.Vendor.Name,
            }).FirstOrDefaultAsync();
            if (requiredAssetDTO == null)
            {
                // Return error if no assets associated to this organization
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No assets found",
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredAssetDTO
            };
        }

        public async Task<ApiResponseDTO> DeleteAsset(string userId, int AssetId)
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

            var userOrganization = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization);
            if (userOrganization == null)
            {
                // Return error if user not associated to any organization
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not associated to any organization."
                    }
                };
            }

            var RequiredAsset = await _applicationDbContext.Assets.FirstOrDefaultAsync(x => x.AssetId == AssetId && x.OrganizationId == userOrganization.OrganizationId);
            if (RequiredAsset == null)
            {
                // Return error if no assets associated to this organization
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No assets found",
                    }
                };
            }

            _applicationDbContext.Assets.Remove(RequiredAsset);
            var savedDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Asset deletion failed.",
                        "Try Again later."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Asset deletion successfully.",
                    }
            };
        }
    }
}