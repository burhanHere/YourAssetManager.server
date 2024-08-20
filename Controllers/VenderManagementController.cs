using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/[controller]")]
    public class VendorManagementController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly VendorManagementRepository _vendorManagementRepository = new(applicationDbContext, userManager);

        // Define the CreateVendor endpoint to create a new vendor for the current user's organization
        [HttpPost("CreateVendor")]
        public async Task<IActionResult> CreateVendor([FromBody]VendorDTO VendorDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrEmpty())
            {
                // If the user ID is not found in the token, return an unauthorized response
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            // Call the repository method to create the vendor with the current user's ID
            ApiResponseDTO result = await _vendorManagementRepository.CreateVendor(userId!, VendorDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the vendor was successfully created
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a MethodNotAllowed response if the operation is not allowed
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the GetAllVendors endpoint to retrieve all vendors for the current user's organization
        [HttpGet("GetAllVendors")]
        public async Task<ApiResponseDTO> GetAllVendors()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrEmpty())
            {
                // If the user ID is not found in the token, return an unauthorized response
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            // Call the repository method to get all vendors associated with the current user
            ApiResponseDTO result = await _vendorManagementRepository.GetAllVendors(userId!);
            // Return the result containing the list of vendors or any error encountered
            return result;
        }

        // Define the UpdateVendor endpoint to update an existing vendor
        [HttpPost("UpdateVendor")]
        public async Task<IActionResult> UpdateVendor([FromBody]VendorDTO VendorUpdate)
        {
            // Call the repository method to update the vendor information
            ApiResponseDTO result = await _vendorManagementRepository.UpdateVendor(VendorUpdate);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the vendor was successfully updated
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the organization to be updated was not found
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the DeleteVendor endpoint to delete an existing vendor by its ID
        [HttpDelete("DeleteVendor")]
        public async Task<IActionResult> DeleteVendor(int VendorId)
        {
            // Call the repository method to delete the vendor by its ID
            ApiResponseDTO result = await _vendorManagementRepository.DeleteVendor(VendorId);

            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the vendor was successfully deleted
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the vendor was not found
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                // Return a MethodNotAllowed response if the operation is not allowed
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the GetVendorById endpoint to retrieve a specific vendor by its ID
        [HttpGet("GetVendorById")]
        public async Task<ApiResponseDTO> GetVendorById(int VendorId)
        {
            // Call the repository method to get the vendor by its ID
            ApiResponseDTO result = await _vendorManagementRepository.GetVendorById(VendorId);
            // Return the result containing the vendor information or any error encountered
            return result;
        }
    }
}