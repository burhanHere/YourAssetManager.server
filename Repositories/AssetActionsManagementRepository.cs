using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Services;

namespace YourAssetManager.Server.Repositories
{
    public class AssetActionsManagementRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, MailSettingsDTO mailSettings)
    {
        private readonly ApplicationDbContext _applicationRepository = applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly EmailService _emailService = new(mailSettings);

        public async Task<ApiResponseDTO> RequestAsset(string currentLogedInUser, AssetRequestDTO assetRequestDTO)
        {
            var targetUserOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == currentLogedInUser);
            if (targetUserOrganization == null)
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
            if (string.IsNullOrEmpty(assetRequestDTO.RequestDescription))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Request Description cant be empty!"
                    }
                };
            }
            AssetRequest newRequest = new()
            {
                RequestDescription = assetRequestDTO.RequestDescription,
                RequestStatus = "PENDING",
                RequesterId = targetUserOrganization.UserId,
                OrganizationId = targetUserOrganization.OrganizationId
            };

            _applicationRepository.AssetRequests.Add(newRequest);
            var savedDbChanges = await _applicationRepository.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>()
                    {
                        "Failed to Create Asset Request."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>()
                    {
                        "Asset Request created successfully."
                    }
            };
        }
        public async Task<ApiResponseDTO> ProcessAssetRequest(string currentLogedInUser, int RequestId, bool action)
        {
            var targetUser = await _userManager.FindByIdAsync(currentLogedInUser);
            if (targetUser == null)
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

            var userOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == targetUser.Id && x.Organization.ActiveOrganization);
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

            AssetRequest targetAssetRequest = await _applicationRepository.AssetRequests.FirstOrDefaultAsync(x => x.OrganizationId == userOrganization.OrganizationId && x.Id == RequestId);
            if (targetAssetRequest == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Target Request not found."
                    }
                };
            }
            if (targetAssetRequest.RequestStatus != "PENDING")
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't perform this action. Request Status Not pending.",
                    }
                };
            }
            string message;
            string emailMessage;
            if (action)
            {
                targetAssetRequest.RequestStatus = "FULFILLED";
                message = "AssetRequest Marked as Fulfilled successfully.";
                emailMessage = @"Your asset Request has been Fulfilled.<br>";
            }
            else
            {
                targetAssetRequest.RequestStatus = "DECLINED";
                message = "AssetRequest Declined successfully.";
                emailMessage = @"Your asset Request has been declined.<br>";
            }
            _applicationRepository.AssetRequests.Update(targetAssetRequest);
            var savedDbChanges = await _applicationRepository.SaveChangesAsync();
            if (savedDbChanges == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to Process Asset request."
                    }
                };
            }
            var requesterUser = await _userManager.FindByIdAsync(targetAssetRequest.RequesterId);
            _ = await _emailService.SendEmailAsync(requesterUser.Email, "Asset Request Decline", emailMessage);
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    message
                }
            };
        }
        public async Task<ApiResponseDTO> AssetAssign(string currentLogedInUser, AssetAssignmentDTO assetAssignmentDTO)
        {
            var targetUser = await _userManager.FindByIdAsync(currentLogedInUser);
            if (targetUser == null)
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
            var userOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == targetUser.Id && x.Organization.ActiveOrganization);
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

            var targetAsset = await _applicationRepository.Assets.FirstOrDefaultAsync(x => x.AssetId == assetAssignmentDTO.AssetId);
            if (targetAsset == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Target Asset not found."
                    }
                };
            }
            if (targetAsset.AssetStatusId != 4)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Asset Not Available.",
                    }
                };
            }

            AssetAssignment newAssetAssignment = new();
            newAssetAssignment.AssignedDate = DateTime.Now;
            newAssetAssignment.Notes = assetAssignmentDTO.Notes;
            newAssetAssignment.AssignedToId = assetAssignmentDTO.AssignedToId;
            newAssetAssignment.AssignedById = targetUser.Id;
            newAssetAssignment.AssetId = assetAssignmentDTO.AssetId;
            newAssetAssignment.AssetRequestId = assetAssignmentDTO.RequestId;

            await _applicationRepository.AssetAssignments.AddAsync(newAssetAssignment);
            targetAsset.AssetStatusId = 1;
            _applicationRepository.Assets.Update(targetAsset);

            var savedDbChanges = await _applicationRepository.SaveChangesAsync();
            if (savedDbChanges < 2)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to Assign Asset"
                    }
                };
            }
            string message = $"<table><thead><tr><th>Asset ID</th><th>Asset Name</th><th>Date</th></tr></thead><tbody><tr><td>{targetAsset.AssetId}</td><td>{targetAsset.AssetName}</td><td>{newAssetAssignment.AssignedDate}</td></tr></tbody></table><br><p>The above-mentioned assets are assigned to you.</p>";
            var requesterUser = await _userManager.FindByIdAsync(assetAssignmentDTO.AssignedToId);
            _ = await _emailService.SendEmailAsync(requesterUser.Email, "Asset Assignent", message);
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Asset Assigned Successfully."
                }
            };
        }
        public async Task<ApiResponseDTO> GetAssetRequestsByUserId(string targetUserId)
        {
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
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
            var userOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == targetUser.Id && x.Organization.ActiveOrganization);
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

            var requiredAssetRequests = await _applicationRepository.AssetRequests
            .Where(x => x.RequesterId == targetUser.Id)
            .Select(x => new
            {
                RequestId = x.Id,
                RequestDescription = x.RequestDescription,
                RequestStatus = x.RequestStatus,
                RequesterId = x.RequesterId,
                RequesterUserName = x.Requester.UserName,
            }).ToListAsync();

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = requiredAssetRequests,
            };
        }
        public async Task<ApiResponseDTO> ReturnAsset(string currentLogedInUser, AssetReturnDTO assetReturnDTO)
        {
            var targetUser = await _userManager.FindByIdAsync(currentLogedInUser);
            if (targetUser == null)
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
            var userOrganization = await _applicationRepository.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == targetUser.Id && x.Organization.ActiveOrganization);
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

            var targetAssetAssignment = await _applicationRepository.AssetAssignments
            .Where(x => x.AssetId == assetReturnDTO.AssetId)
            .OrderByDescending(x => x.AssignedDate)
            .Select(x => new
            {
                Id = x.Id,
                AssignedToId = x.AssignedToId,
                Asset = x.Asset
            }).FirstOrDefaultAsync();

            if (targetAssetAssignment == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Corresponding Asset Assignemnt not found."
                    }
                };
            }
            AssetReturn newAssetReturnEntry = new()
            {
                ReturnedDate = DateTime.Now,
                ReturnCondition = assetReturnDTO.ReturnCondition,
                Notes = assetReturnDTO.Notes,
                AssetAssignmentId = targetAssetAssignment.Id
            };

            _applicationRepository.AssetReturns.AddAsync(newAssetReturnEntry);
            targetAssetAssignment.Asset.AssetStatusId = 4;
            _applicationRepository.Assets.Update(targetAssetAssignment.Asset);

            var savedDbChanges = await _applicationRepository.SaveChangesAsync();
            if (savedDbChanges < 2)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed To return asset."
                    }
                };
            }
            string message = $"<table><thead><tr><th>Asset ID</th></tr></thead><tbody><tr><td>{targetAssetAssignment.Asset.AssetId}</td></tr></tbody></table><br><p>The above-mentioned assets has been submitted back to Asset's Departemnt</p>";
            var requesterUser = await _userManager.FindByIdAsync(targetAssetAssignment.AssignedToId);
            _ = await _emailService.SendEmailAsync(requesterUser.Email, "Asset Return", message);
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Asset returned successfully."
                }
            };
        }
    }
}