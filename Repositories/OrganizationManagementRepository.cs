using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    /// <summary>
    /// Repository for handling organization-related tasks.
    /// </summary>
    public class OrganizationManagementRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
    {
        // Fields for database context and user manager
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        /// <summary>
        /// Retrieves the organizations associated with the signed-in user.
        /// </summary>
        /// <param name="SignedInUserName">The username of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponceDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponceDTO> GetOrganizationsInfo(string SignedInUserName)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                // Return error if user not found
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

            // Query the organizations related to the user
            var resultontOrganization = await _applicationDbContext.Organizations
                                .Select(x => x)
                                .Where(x => x.ApplicationUserId == user.Id && x.ActiveOrganization == true)
                                .ToListAsync();

            if (resultontOrganization.IsNullOrEmpty())
            {
                // Return success but indicate no organizations found
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "No Organization associated to this user.",
                        "Please create an organization first."
                    }
                };
            }

            // Return the list of organizations
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new
                {
                    organization = resultontOrganization
                }
            };
        }

        /// <summary>
        /// Creates a new organization for the signed-in user.
        /// </summary>
        /// <param name="newOrganization">The new organization's data.</param>
        /// <param name="SignedInUserName">The username of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponceDTO"/> indicating the status of the operation.</returns>

        public async Task<ApiResponceDTO> CreateOrganization(OrganizationDTO newOrganization, string SignedInUserName)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                // Return error if user not found
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

            // Check if any organization is already registerd to this user and is active or not
            var organizations = await _applicationDbContext.Organizations.Select(x => x).Where(x => x.ApplicationUserId == user.Id && x.ActiveOrganization == true).ToListAsync();
            if (organizations.Count > 0)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status409Conflict,
                    ResponceData = new List<string>
                    {
                       "An account with this email address already exists."
                    }
                };
            }

            // Check if an organization with the same name already exists
            var sameOrganizationNames = await _applicationDbContext.Organizations
                                            .FirstOrDefaultAsync(x => x.OrganizationName == newOrganization.OrganizationName);
            if (sameOrganizationNames != null)
            {
                // Return error if organization name is not unique
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

            // Check if an organization with the same domain already exists
            var organizationDomainUniqueness = await _applicationDbContext.Organizations
                                            .FirstOrDefaultAsync(x => x.OrganizationDomain == newOrganization.OrganizationDomain);
            if (organizationDomainUniqueness != null)
            {
                // Return error if organization domain is not unique
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

            // Create the new organization entity
            Organization organization = new()
            {
                OrganizationName = newOrganization.OrganizationName,
                Description = newOrganization.Description,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                OrganizationDomain = newOrganization.OrganizationDomain,
                ActiveOrganization = true,
                ApplicationUserId = user.Id,
            };

            // Add the new organization to the database
            await _applicationDbContext.Organizations.AddAsync(organization);
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();

            if (saveDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "New Organization creation failed."
                    }
                };
            }

            // Return success if the organization was created successfully
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new List<string>
                    {
                        "New Organization created successfully."
                    }
            };
        }

        /// <summary>
        /// Updates the details of an existing organization.
        /// </summary>
        /// <param name="newOrganization">The organization's updated data.</param>
        /// <param name="SignedInUserName">The username of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponceDTO"/> indicating the status of the operation.</returns>

        public async Task<ApiResponceDTO> UpdateOrganization(OrganizationDTO newOrganization, string SignedInUserName)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(SignedInUserName);
            if (user == null)
            {
                // Return error if user not found
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

            // Find the organization by name
            var organization = await _applicationDbContext.Organizations
                                            .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id && x.ActiveOrganization == true);
            if (organization == null)
            {
                // Return error if organization not found
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

            // Update organization properties with new values if provided
            organization.OrganizationName = newOrganization.OrganizationName.IsNullOrEmpty() ? organization.OrganizationName : newOrganization.OrganizationName;

            organization.Description = newOrganization.Description.IsNullOrEmpty() ? organization.Description : newOrganization.Description;

            organization.OrganizationDomain = newOrganization.OrganizationDomain.IsNullOrEmpty() ? organization.OrganizationDomain : newOrganization.OrganizationDomain;

            organization.UpdatedDate = DateTime.Now;

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving changes failed
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "Updating Organization detials failed."
                    }
                };
            }

            // Return success if the organization was updated successfully
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new List<string>
                    {
                        "Organization details updated successfully."
                    }
            };
        }
        public async Task<ApiResponceDTO> DeactivateOrganization(string OrganizationName, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {

                // Return error if user not found
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
            var organization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id && x.OrganizationName == OrganizationName && x.ActiveOrganization);
            if (organization == null)
            {
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponceData = new List<string>
                    {
                        "Organization not found."
                    }
                };
            }
            organization.ActiveOrganization = false;
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving changes failed
                return new ApiResponceDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponceData = new List<string>
                    {
                        "Unable to deactivate organizatino."
                    }
                };
            }

            // Return success if the organization was updated successfully
            return new ApiResponceDTO
            {
                Status = StatusCodes.Status200OK,
                ResponceData = new List<string>
                    {
                        "Organization Deactivated Successfully."
                    }
            };
        }
    }
}