using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Controllers
{
    public class OrganizationRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
    {
        ApplicationDbContext _applicationDbContext = applicationDbContext;
        UserManager<ApplicationUser> _userManager = userManager;
        public async Task<ApiResponceDTO> GetMyOrganizations(string SignedInUserName)
        {
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            var resultontOrganizations = await _applicationDbContext.Organizations
                                .Select(x => x)
                                .Where(x => x.ApplicationUserId == user.Id)
                                .ToListAsync();

            if (resultontOrganizations == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status200OK,
                    ResponceData = new List<string>
                    {
                        "No Organization associated to this user."
                    }
                };
            }
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new
                {
                    count = resultontOrganizations.Count,
                    organizationList = resultontOrganizations
                }
            };
        }
        public async Task<ApiResponceDTO> CreateOrganization(OrganizationDTO newOrganization, string SignedInUserName)
        {
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            var sameOrganizationNames = await _applicationDbContext.Organizations
                                            .FirstOrDefaultAsync(x => x.OrganizationName == newOrganization.OrganizationName);
            if (sameOrganizationNames != null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "Organization Name must be unique.",
                        "An other Organization exists with the same Name."
                    }
                };
            }
            var organizationDomainUniqueness = await _applicationDbContext.Organizations
                                            .FirstOrDefaultAsync(x => x.OrganizationDomain == newOrganization.OrganizationDomain);
            if (organizationDomainUniqueness != null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "Organization Domain must be unique.",
                        "An other Organization exists with the same Domain."
                    }
                };
            }

            Organization organization = new()
            {
                OrganizationName = newOrganization.OrganizationName,
                Description = newOrganization.Description,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                OrganizationDomain = newOrganization.OrganizationDomain,
                ApplicationUserId = user.Id,
            };
            await _applicationDbContext.Organizations.AddAsync(organization);
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "New Organization creation failed."
                    }
                };
            }
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new List<string>
                    {
                        "New Organization created successfully."
                    }
            };
        }
        public async Task<ApiResponceDTO> UpdateOrganization(OrganizationDTO newOrganization, string SignedInUserName)
        {
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            var organization = await _applicationDbContext.Organizations
                                            .FirstOrDefaultAsync(x => x.OrganizationName == newOrganization.OrganizationName);
            if (organization == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid Organization Name",
                        "Organization Not found."
                    }
                };
            }
            organization.OrganizationName = newOrganization.OrganizationName.IsNullOrEmpty() ? organization.OrganizationName : newOrganization.OrganizationName;
            organization.Description = newOrganization.Description.IsNullOrEmpty() ? organization.Description : newOrganization.Description;
            organization.OrganizationDomain = newOrganization.OrganizationDomain.IsNullOrEmpty() ? organization.OrganizationDomain : newOrganization.OrganizationDomain;
            organization.UpdatedDate = DateTime.Now;

            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "Updating Organization detials failed."
                    }
                };
            }
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new List<string>
                    {
                        "Organization details updated successfully."
                    }
            };
        }

        public async Task<ApiResponceDTO> OrganizationOwnerDetails(string SignedInUserName)
        {
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Invalid user request.",
                        "User not found."
                    }
                };
            }
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new
                {
                    OrganizationOwnerData = user
                }
            };
        }
    }
}