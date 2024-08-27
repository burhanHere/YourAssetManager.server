using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ApiResponseDTO> Search(string query, string tables)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = "Search query cannot be empty."
                };
            }

            // Split the tables parameter into an array
            var tableList = tables?.Split(',').Select(t => t.Trim().ToLower()).ToList();

            var results = new
            {
                Assets = tableList == null || tableList.Contains("assets")
                    ? await _applicationDbContext.Assets.Where(a => a.AssetName.Contains(query)).ToListAsync()
                    : null,
                // AssetAssignments = tableList == null || tableList.Contains("assetassignments")
                //     ? await _applicationDbContext.AssetAssignments.Where(a => a.Notes.Contains(query)).ToListAsync()
                //     : null,
                AssetCategories = tableList == null || tableList.Contains("categories")
                    ? await _applicationDbContext.AssetCategories.Where(a => a.CategoryName.Contains(query)).ToListAsync()
                    : null,
                // AssetMaintenances = tableList == null || tableList.Contains("assetmaintenances")
                //     ? await _applicationDbContext.AssetMaintenances.Where(a => a.Description.Contains(query)).ToListAsync()
                //     : null,
                // AssetRequests = tableList == null || tableList.Contains("assetrequests")
                //     ? await _applicationDbContext.AssetRequests.Where(a => a.RequestDescription.Contains(query)).ToListAsync()
                //     : null,
                // AssetRetires = tableList == null || tableList.Contains("assetretires")
                //     ? await _applicationDbContext.AssetRetires.Where(a => a.RetirementReason.Contains(query)).ToListAsync()
                //     : null,
                // AssetReturns = tableList == null || tableList.Contains("assetreturns")
                //     ? await _applicationDbContext.AssetReturns.Where(a => a.ReturnCondition.Contains(query) || a.Notes.Contains(query)).ToListAsync()
                //     : null,
                AssetTypes = tableList == null || tableList.Contains("assettypes")
                    ? await _applicationDbContext.AssetTypes.Where(a => a.AssetTypeName.Contains(query) || a.Description.Contains(query)).ToListAsync()
                    : null,
                // Organizations = tableList == null || tableList.Contains("organizations")
                //     ? await _applicationDbContext.Organizations.Where(o => o.OrganizationName.Contains(query) || o.Description.Contains(query)).ToListAsync()
                //     : null,
                // OrganizationDomains = tableList == null || tableList.Contains("organizationdomains")
                //     ? await _applicationDbContext.OrganizationDomains.Where(o => o.OrganizationDomainString.Contains(query)).ToListAsync()
                //     : null,
                Vendors = tableList == null || tableList.Contains("vendors")
                    ? await _applicationDbContext.Vendors.Where(v => v.Name.Contains(query) || v.PhoneNumber.Contains(query)).ToListAsync()
                    : null,
                Users = tableList == null || tableList.Contains("users")
                    ? await _applicationDbContext.Users.Where(u => u.UserName.Contains(query) || u.Email.Contains(query)).ToListAsync()
                    : null
            };

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = results
            };
        }
    }
}