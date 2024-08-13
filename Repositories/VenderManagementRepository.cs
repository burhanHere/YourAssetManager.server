using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class VenderManagementRepository(ApplicationDbContext applicationDbContext)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        public async Task<ApiResponseDTO> CreateVender(string userId, VenderDTO venderDTO)
        {
            var userOrganization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ActiveOrganization == true);
            if (userOrganization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't Create a vender as no organization is associated to this user.",
                        "Please create an organization first."
                    }
                };
            }

            Vender newVender = new()
            {
                Name = venderDTO.Name,
                Email = venderDTO.Email,
                OfficeAddress = venderDTO.OfficeAddress,
                PhoneNumber = venderDTO.PhoneNumber,
                OrganizationId = userOrganization.Id,
            };

            await _applicationDbContext.Venders.AddAsync(newVender);
            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving to the database failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "New Vender creation failed."
                    }
                };
            }
            // Return success if the organization was created successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "New Vender created successfully."
                    }
            };
        }
        public async Task<ApiResponseDTO> GetAllVenders(string userId)
        {
            var userOrganization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ActiveOrganization == true);
            if (userOrganization == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Can't Create a vender as no organization is associated to this user.",
                        "Please create an organization first."
                    }
                };
            }
            var requiredVendes = await _applicationDbContext.Venders.Where(x => x.OrganizationId == userOrganization.Id).ToListAsync();
            if (requiredVendes.Count == 0)
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
            // converting to DTO
            List<VenderDTO> venderList = [];
            foreach (var item in requiredVendes)
            {
                venderList.Add(new VenderDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    OfficeAddress = item.OfficeAddress,
                    PhoneNumber = item.PhoneNumber
                });
            }

            // Return the list of venders
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = venderList
            };
        }
        public async Task<ApiResponseDTO> GetVenderById(int venderId)
        {
            // Find the vender by its ID
            Vender requiredVender = await _applicationDbContext.Venders.FirstOrDefaultAsync(x => x.Id == venderId);

            // Check if the vender exists
            if (requiredVender == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Vender not found."
                    }
                };
            }

            // Convert to DTO
            VenderDTO venderDTO = new VenderDTO
            {
                Id = requiredVender.Id,
                Name = requiredVender.Name,
                Email = requiredVender.Email,
                OfficeAddress = requiredVender.OfficeAddress,
                PhoneNumber = requiredVender.PhoneNumber
            };

            // Return the vender DTO
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = venderDTO
            };
        }
        public async Task<ApiResponseDTO> UpdateVender(VenderDTO venderUpdate)
        {
            Vender requiredVender = await _applicationDbContext.Venders.FirstOrDefaultAsync(x => x.Id == venderUpdate.Id);
            if (requiredVender == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Unable to update venderas no vender have this id"
                    }
                };
            }

            requiredVender.Name = venderUpdate.Name.IsNullOrEmpty() ? requiredVender.Name : venderUpdate.Name;
            requiredVender.Email = venderUpdate.Email.IsNullOrEmpty() ? requiredVender.Email : venderUpdate.Email;
            requiredVender.OfficeAddress = venderUpdate.OfficeAddress.IsNullOrEmpty() ? requiredVender.OfficeAddress : venderUpdate.OfficeAddress;
            requiredVender.PhoneNumber = venderUpdate.PhoneNumber.IsNullOrEmpty() ? requiredVender.PhoneNumber : venderUpdate.PhoneNumber;

            var saveDbChanges = await _applicationDbContext.SaveChangesAsync();
            if (saveDbChanges == 0)
            {
                // Return error if saving changes failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Updating Vender detials failed."
                    }
                };
            }

            // Return success if the Vender was updated successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        "Vender details updated successfully."
                    }
            };
        }
        public async Task<ApiResponseDTO> DeleteVender(int venderId)
        {
            // Find the vender by its ID
            Vender venderToDelete = await _applicationDbContext.Venders.FirstOrDefaultAsync(x => x.Id == venderId);

            // Check if the vender exists
            if (venderToDelete == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Vender not found."
                    }
                };
            }
            var associatedAsset = await _applicationDbContext.Assets.AnyAsync(x => x.VenderId == venderToDelete.Id);
            if (associatedAsset)
            {
                // Return error if Vender is associated with assets
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status405MethodNotAllowed,
                    ResponseData = new List<string>
                    {
                        "Cannot delete Vender as it is associated with one or more assets."
                    }
                };
            }

            // Remove the vender from the database
            _applicationDbContext.Venders.Remove(venderToDelete);

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
                        "Failed to delete vender."
                    }
                };
            }

            // Return success if the vender was deleted successfully
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Vender deleted successfully."
                }
            };
        }
    }
}