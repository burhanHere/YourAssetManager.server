using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Route("YourAssetManager.Serverg/{controller}")]
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    public class OrganizationManagementController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly OrganizationManagementRepository _organizationManagementRepository = new(applicationDbContext, userManager);

        // Define the GetMyOrganizations endpoint to retrieve all organizations related to the currently signed-in user
        [HttpGet("/GetOrganizationsInfo")]
        public async Task<ApiResponseDTO> GetOrganizationsInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            // Call the repository method to get organizations related to the current user
            ApiResponseDTO result = await _organizationManagementRepository.GetOrganizationsInfo(userId);
            return result;
        }

        // Define the CreateOrganization endpoint to create a new organization for the current user
        [HttpPost("/CreateOrganization")]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationDTO newOrganization)
        {
            // Call the repository method to create a new organization with the current user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _organizationManagementRepository.CreateOrganization(newOrganization, userId);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the organization was successfully created
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if something went wrong and the organization was not created
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status409Conflict)
            {
                return Conflict(result);
            }
            else if (result.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the UpdateOrganization endpoint to update an existing organization for the current user
        [HttpPut("/UpdateOrganization")]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationDTO newOrganization)
        {
            // Call the repository method to update the organization with the current user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _organizationManagementRepository.UpdateOrganization(newOrganization, userId);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the organization was successfully updated
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

        // Define the DeleteOrganization endpoint to delete and organization
        [HttpDelete("/DeleteOrganization")]
        public async Task<IActionResult> DeleteOrganization()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _organizationManagementRepository.DeactivateOrganization(userId);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if organization is deactivated
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return an NotFound response if organization or user is not found
                return NotFound(result);
            }
            // Return an BadRequest response if organization deactivativation failed
            return BadRequest(result);
        }
    }
}