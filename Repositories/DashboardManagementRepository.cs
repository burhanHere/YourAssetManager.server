using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;

namespace YourAssetManager.Server.Models
{
    public class DashboardManagementRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<ApiResponseDTO> GetDashBoardStatiticsData(string signedInUserId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(signedInUserId);
            if (user == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }

            // Check if the user already has an active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);
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

            int CatagoryCount = await _applicationDbContext.AssetCategories.CountAsync(x => x.CategoryOrganizationId == userOrganization.OrganizationId);

            int vendorCount = await _applicationDbContext.Vendors.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId);

            int AssetTypeCount = await _applicationDbContext.AssetTypes.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId);

            int AssetCount = await _applicationDbContext.Assets.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId);

            var tempAssetStatusIdNameList = await _applicationDbContext.AssetStatuses.ToListAsync();

            var assetCountByStatus = new Dictionary<string, int>();
            int temp = 0;
            foreach (var status in tempAssetStatusIdNameList)
            {
                temp = await _applicationDbContext.Assets.CountAsync(x => x.AssetStatusId == status.Id);
                assetCountByStatus.Add(status.StatusName, temp);
                temp = 0;
            }

            var tempCatagoryIdNameList = await _applicationDbContext.AssetCategories.Where(x => x.CategoryOrganizationId == userOrganization.Id).Select(x => new { x.Id, x.CategoryName }).ToListAsync();
            var assetCountByCatagoryNames = new Dictionary<string, int>();
            foreach (var item in tempCatagoryIdNameList)
            {
                temp = await _applicationDbContext.Assets.CountAsync(x => x.OrganizationId == userOrganization.OrganizationId && x.AssetCategoryId == item.Id);
                assetCountByCatagoryNames.Add(item.CategoryName, temp);
                temp = 0;
            }

            var resultData = new
            {
                vendorCount = vendorCount,
                CatagoryCount = CatagoryCount,
                AssetTypeCount = AssetTypeCount,
                AssetCount = AssetCount,
                AssetCountByStatus = assetCountByStatus.Select(item => new
                {
                    name = item.Key,
                    value = item.Value
                }).ToList(),
                AssetCountByCatagory = assetCountByCatagoryNames.Select(item => new
                {
                    name = item.Key,
                    value = item.Value
                }).ToList()
            };
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = resultData
            };
        }
        public async Task<ApiResponseDTO> GetAllAssetRequests(string signedInUserId, string? requiredRequestsStatus = null)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(signedInUserId);
            if (user == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }

            // Check if the user already has an active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);
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

            var requiredAssetrequests = await _applicationDbContext.AssetRequests
            .Where(x => x.OrganizationId == userOrganization.OrganizationId && x.RequestStatus == (requiredRequestsStatus ?? x.RequestStatus))
            .Select(x => new
            {
                RequestId = x.Id,
                RequestDescription = x.RequestDescription,
                RequestStatus = x.RequestStatus,
                RequesterId = x.RequesterId,
                RequesterUserName = x.Requester.UserName,

            })
            .ToListAsync();
            if (requiredAssetrequests.Count == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No request found in."
                    }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredAssetrequests
            };
        }
        public async Task<ApiResponseDTO> Search(string assetQuery, int assetCatagoriId)
        {
            if (string.IsNullOrEmpty(assetQuery))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = "Search query cannot be empty."
                };
            }

            List<AssetDTO> targetAssets;
            if (assetCatagoriId <= 0)
            {
                targetAssets = await _applicationDbContext.Assets
                .Where(a => a.AssetName.Contains(assetQuery) || a.Description.Contains(assetQuery))
                .Select(x => new AssetDTO()
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
                    OrganizationData = x.Organization.OrganizationName,
                    AssetStatusData = x.AssetStatus.StatusName,
                    AssetCategoryData = x.AssetCategory.CategoryName,
                    AssetTypeData = x.AssetType.AssetTypeName,
                    VendorData = x.Vendor.Name
                }).ToListAsync();
            }
            else
            {
                targetAssets = await _applicationDbContext.Assets
                .Where(a => a.AssetName.Contains(assetQuery) || a.Description.Contains(assetQuery) || a.AssetCategoryId == assetCatagoriId)
                .Select(x => new AssetDTO()
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
                    OrganizationData = x.Organization.OrganizationName,
                    AssetStatusData = x.AssetStatus.StatusName,
                    AssetCategoryData = x.AssetCategory.CategoryName,
                    AssetTypeData = x.AssetType.AssetTypeName,
                    VendorData = x.Vendor.Name
                }).ToListAsync();
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = targetAssets
            };
        }
    }
}