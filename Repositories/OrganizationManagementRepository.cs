using Microsoft.AspNetCore.Identity;
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
    public class OrganizationManagementRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
    {
        // Fields for database context and user manager
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        /// <summary>
        /// Retrieves the organizations associated with the signed-in user.
        /// </summary>
        /// <param name="SignedInUserName">The username of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetOrganizationsInfo(string SignedInUserId)
        {
            // Find the user by username
            var user = await _userManager.FindByIdAsync(SignedInUserId);
            if (user == null)
            {
                // Return error if user not found
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

            var userOrganizations = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);
            if (userOrganizations == null)
            {
                // Return success but indicate no organizations found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No Organization associated to this user.",
                        "Please create an organization first."
                    }
                };
            }
            var requiredOrganization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.Id == userOrganizations.OrganizationId);
            // converting to DTO
            OrganizationDTO organizationsDTO = new()
            {
                OrganizationName = requiredOrganization.OrganizationName,
                Description = requiredOrganization.Description,
                OrganizationDomain = requiredOrganization.OrganizationDomain
            };

            // Return the list of organizations
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new
                {
                    organizations = organizationsDTO
                }
            };
        }

        /// <summary>
        /// Creates a new organization for the signed-in user.
        /// </summary>
        /// <param name="newOrganization">The new organization's data.</param>
        /// <param name="SignedInUserName">The usernamh.e of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> CreateOrganization(OrganizationDTO newOrganization, string signedInUserId)
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
            var existingOrganizations = await _applicationDbContext.UserOrganizations
                .AnyAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);

            if (existingOrganizations)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status409Conflict,
                    ResponseData = new List<string>
                    {
                        "An organization has already been created by this user.",
                        "To create a new organization, deactivate the existing one."
                    }
                };
            }

            // Check if an organization with the same name already exists
            var existingOrganizationByName = await _applicationDbContext.Organizations
                .FirstOrDefaultAsync(x => x.OrganizationName == newOrganization.OrganizationName);

            if (existingOrganizationByName != null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Organization name must be unique.",
                        "Another organization exists with the same name."
                    }
                };
            }

            // Check if an organization with the same domain already exists
            var existingOrganizationByDomain = await _applicationDbContext.Organizations
                .FirstOrDefaultAsync(x => x.OrganizationDomain == newOrganization.OrganizationDomain);

            if (existingOrganizationByDomain != null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Organization domain must be unique.",
                        "Another organization exists with the same domain."
                    }
                };
            }

            // Create the new organization
            var organization = new Organization
            {
                OrganizationName = newOrganization.OrganizationName,
                Description = newOrganization.Description,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                OrganizationDomain = newOrganization.OrganizationDomain,
                ActiveOrganization = true,
            };

            // Add the organization to the database
            await _applicationDbContext.Organizations.AddAsync(organization);
            var saveChangesResult = await _applicationDbContext.SaveChangesAsync();
            if (saveChangesResult == 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to create new organization."
                    }
                };
            }
            var userOrganization = new UserOrganization
            {
                UserId = user.Id,
                OrganizationId = organization.Id
            };
            await _applicationDbContext.UserOrganizations.AddAsync(userOrganization);
            saveChangesResult = await _applicationDbContext.SaveChangesAsync();
            if (saveChangesResult == 0)
            {
                _applicationDbContext.Organizations.Remove(organization);
                _applicationDbContext.SaveChanges();
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to create new organization."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "New organization created successfully."
                }
            };
        }

        /// <summary>
        /// Updates the details of an existing organization.
        /// </summary>
        /// <param name="newOrganization">The organization's updated data.</param>
        /// <param name="SignedInUserName">The username of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> UpdateOrganization(OrganizationDTO updatedOrganization, string signedInUserId)
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

            // Find the organization IDs related to the user
            var userOrganizations = await _applicationDbContext.UserOrganizations.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);

            if (userOrganizations == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No organization found for the user."
                    }
                };
            }

            // Find the active organization to update
            var organizationToUpdate = await _applicationDbContext.Organizations
                .FirstOrDefaultAsync(x => x.ActiveOrganization && x.Id == userOrganizations.Id);

            if (organizationToUpdate == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Active organization not found."
                    }
                };
            }

            // Update organization properties with new values if provided
            if (!string.IsNullOrEmpty(updatedOrganization.OrganizationName))
            {
                organizationToUpdate.OrganizationName = updatedOrganization.OrganizationName;
            }

            if (!string.IsNullOrEmpty(updatedOrganization.Description))
            {
                organizationToUpdate.Description = updatedOrganization.Description;
            }

            if (!string.IsNullOrEmpty(updatedOrganization.OrganizationDomain))
            {
                organizationToUpdate.OrganizationDomain = updatedOrganization.OrganizationDomain;
            }

            organizationToUpdate.UpdatedDate = DateTime.UtcNow;

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges <= 0)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to update organization details."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Organization details updated successfully."
                }
            };
        }

        /// <summary>
        /// Deactivates the active organization associated with the signed-in user.
        /// </summary>
        /// <param name="SignedInUserId">The ID of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> DeactivateOrganization(string SignedInUserId)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(SignedInUserId);
            if (user == null)
            {
                // Return error if the user is not found
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

            // Query the organizations related to the user
            var userOrganizations = await _applicationDbContext.UserOrganizations
                                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Organization.ActiveOrganization == true);

            if (userOrganizations == null)
            {
                // Return success but indicate no organizations found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No Organization associated to this user.",
                        "Please create an organization first."
                    }
                };
            }
            var resultentOrganization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.Id == userOrganizations.OrganizationId && x.ActiveOrganization == true);
            if (resultentOrganization == null)
            {
                // Return success but indicate no organizations found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No Organization associated to this user.",
                        "Please create an organization first."
                    }
                };
            }
            resultentOrganization.ActiveOrganization = false;

            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving changes failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to deactivate organizatino."
                    }
                };
            }

            // Return success if the organization was updated successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Organization Deactivated Successfully."
                    }
            };
        }
    }
}