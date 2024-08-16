using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    /// <summary>
    /// Repository for handling vendor-related tasks.
    /// </summary>
    public class VendorManagementRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
    {
        // Field for database context
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        /// <summary>
        /// Creates a new vendor for the signed-in user's organization.
        /// </summary>
        /// <param name="userId">The ID of the signed-in user.</param>
        /// <param name="VendorDTO">The new vendor's data.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> CreateVendor(string userId, VendorDTO VendorDTO)
        {
            // Find the user
            var user = await _userManager.FindByIdAsync(userId);
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

            // Find the user's active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
            .FirstOrDefaultAsync(uo => uo.UserId == user.Id && uo.Organization.ActiveOrganization);

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

            // Create the new vendor entity
            Vendor newVendor = new()
            {
                Name = VendorDTO.Name,
                Email = VendorDTO.Email,
                OfficeAddress = VendorDTO.OfficeAddress,
                PhoneNumber = VendorDTO.PhoneNumber,
                OrganizationId = userOrganization.OrganizationId,
            };

            // Add the new vendor to the database
            await _applicationDbContext.Vendors.AddAsync(newVendor);
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "New vendor creation failed."
                    }
                };
            }

            // Return success if the vendor was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "New vendor created successfully."
                }
            };
        }

        /// <summary>
        /// Retrieves all vendors associated with the signed-in user's organization.
        /// </summary>
        /// <param name="userId">The ID of the signed-in user.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetAllVendors(string userId)
        {
            // Find the user
            var user = await _userManager.FindByIdAsync(userId);
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
            // Find the user's active organization
            var userOrganization = await _applicationDbContext.UserOrganizations
                .FirstOrDefaultAsync(uo => uo.UserId == user.Id && uo.Organization.ActiveOrganization);

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

            // Query the vendors associated with the organization
            var requiredVendes = await _applicationDbContext.Vendors.Where(x => x.OrganizationId == userOrganization.OrganizationId).ToListAsync();
            if (requiredVendes.Count == 0)
            {
                // Return success but indicate no vendors found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "No vendors found associated with this organization.",
                        "Please add a vendor first."
                    }
                };
            }

            // Convert to DTO
            List<VendorDTO> VendorList = new();
            foreach (var item in requiredVendes)
            {
                VendorList.Add(new VendorDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    OfficeAddress = item.OfficeAddress,
                    PhoneNumber = item.PhoneNumber
                });
            }

            // Return the list of vendors
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = VendorList
            };
        }

        /// <summary>
        /// Retrieves a vendor by its ID.
        /// </summary>
        /// <param name="VendorId">The ID of the vendor.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> GetVendorById(int VendorId)
        {
            // Find the vendor by its ID
            Vendor requiredVendor = await _applicationDbContext.Vendors.FirstOrDefaultAsync(x => x.Id == VendorId);

            // Check if the vendor exists
            if (requiredVendor == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Vendor not found."
                    }
                };
            }

            // Convert to DTO
            VendorDTO VendorDTO = new VendorDTO
            {
                Id = requiredVendor.Id,
                Name = requiredVendor.Name,
                Email = requiredVendor.Email,
                OfficeAddress = requiredVendor.OfficeAddress,
                PhoneNumber = requiredVendor.PhoneNumber
            };

            // Return the vendor DTO
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = VendorDTO
            };
        }

        /// <summary>
        /// Updates the details of an existing vendor.
        /// </summary>
        /// <param name="VendorUpdate">The vendor's updated data.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> UpdateVendor(VendorDTO VendorUpdate)
        {
            // Find the vendor by its ID
            Vendor requiredVendor = await _applicationDbContext.Vendors.FirstOrDefaultAsync(x => x.Id == VendorUpdate.Id);
            if (requiredVendor == null)
            {
                // Return error if vendor not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Unable to update vendor as no vendor has this ID."
                    }
                };
            }

            // Update vendor properties with new values if provided
            requiredVendor.Name = VendorUpdate.Name.IsNullOrEmpty() ? requiredVendor.Name : VendorUpdate.Name;
            requiredVendor.Email = VendorUpdate.Email.IsNullOrEmpty() ? requiredVendor.Email : VendorUpdate.Email;
            requiredVendor.OfficeAddress = VendorUpdate.OfficeAddress.IsNullOrEmpty() ? requiredVendor.OfficeAddress : VendorUpdate.OfficeAddress;
            requiredVendor.PhoneNumber = VendorUpdate.PhoneNumber.IsNullOrEmpty() ? requiredVendor.PhoneNumber : VendorUpdate.PhoneNumber;

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving changes failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Updating vendor details failed."
                    }
                };
            }

            // Return success if the vendor was updated successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Vendor details updated successfully."
                }
            };
        }

        /// <summary>
        /// Deletes a vendor by its ID.
        /// </summary>
        /// <param name="VendorId">The ID of the vendor to delete.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the operation.</returns>
        public async Task<ApiResponseDTO> DeleteVendor(int VendorId)
        {
            // Find the vendor by its ID
            Vendor VendorToDelete = await _applicationDbContext.Vendors.FirstOrDefaultAsync(x => x.Id == VendorId);

            // Check if the vendor exists
            if (VendorToDelete == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Vendor not found."
                    }
                };
            }

            // Check if the vendor is associated with any assets
            var associatedAsset = await _applicationDbContext.Assets.AnyAsync(x => x.VendorId == VendorToDelete.Id);
            if (associatedAsset)
            {
                // Return error if the vendor is associated with assets
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Cannot delete vendor as it is associated with one or more assets."
                    }
                };
            }

            // Remove the vendor from the database
            _applicationDbContext.Vendors.Remove(VendorToDelete);

            // Save changes to the database
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving changes failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Failed to delete vendor."
                    }
                };
            }

            // Return success if the vendor was deleted successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Vendor deleted successfully."
                }
            };
        }
    }
}
