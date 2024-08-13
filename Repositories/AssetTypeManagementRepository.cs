using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetTypeRepository(ApplicationDbContext applicationDbContext)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        // Create AssetType
        public async Task<ApiResponseDTO> CreateAssetType(string userId, AssetTypeDTO assetType)
        {
            var userOrganization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);
            if (userOrganization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't Create a AssetType as no organization is associated to this user.",
                        "Please create an organization first."
                    }
                };
            }
            AssetType newAssetType = new()
            {
                AssetTypeName = assetType.AssetTypeName,
                Description = assetType.Description,
                OrganizationId = userOrganization.Id,
            };
            await _applicationDbContext.AssetTypes.AddAsync(newAssetType);

            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string> { "Asset Type creation failed." }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string> { "Asset Type created successfully." }
            };
        }
        // Get All AssetTypes
        public async Task<ApiResponseDTO> GetAllAssetTypes(string userId)
        {
            var userOrganization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ActiveOrganization == true);
            if (userOrganization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "There is not vender as nmo organization is associated to this user.",
                        "Please create an organization first."
                    }
                };
            }

            var assetTypes = await _applicationDbContext.AssetTypes.Where(x => x.OrganizationId == userOrganization.Id).ToListAsync();
            if (assetTypes.Count == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "No Asset Types found." }
                };
            }

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

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = assetTypeDTOList
            };
        }
        // Get AssetType By Id
        public async Task<ApiResponseDTO> GetAssetTypeById(int assetTypeId)
        {
            var assetType = await _applicationDbContext.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetTypeId);

            if (assetType == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "Asset Type not found." }
                };
            }

            var assetTypeDTO = new AssetTypeDTO
            {
                Id = assetType.Id,
                AssetTypeName = assetType.AssetTypeName,
                Description = assetType.Description
            };

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = assetTypeDTO
            };
        }
        // Update AssetType
        public async Task<ApiResponseDTO> UpdateAssetType(AssetTypeDTO assetTypeUpdate)
        {
            var existingAssetType = await _applicationDbContext.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetTypeUpdate.Id);

            if (existingAssetType == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "Asset Type not found." }
                };
            }

            existingAssetType.AssetTypeName = assetTypeUpdate.AssetTypeName.IsNullOrEmpty() ? existingAssetType.AssetTypeName : assetTypeUpdate.AssetTypeName;
            existingAssetType.Description = assetTypeUpdate.Description.IsNullOrEmpty() ? existingAssetType.Description : assetTypeUpdate.Description;

            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string> { "Asset Type update failed." }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string> { "Asset Type updated successfully." }
            };
        }
        // Delete AssetType
        public async Task<ApiResponseDTO> DeleteAssetType(int assetTypeId)
        {
            var assetType = await _applicationDbContext.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetTypeId);
            if (assetType == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string> { "Asset Type not found." }
                };
            }

            var associatedAsset = await _applicationDbContext.Assets.AnyAsync(x => x.AssetTypeId == assetType.Id);
            if (associatedAsset)
            {
                // Return error if Vender is associated with assets
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Cannot delete AssetType as it is associated with one or more assets."
                    }
                };
            }

            _applicationDbContext.AssetTypes.Remove(assetType);
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string> { "Asset Type deletion failed." }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string> { "Asset Type deleted successfully." }
            };
        }
    }
}