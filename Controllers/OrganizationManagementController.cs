using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Route("YourAssetManager.Server /{controller}")]
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    public class OrganizationManagementController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly OrganizationManagementRepository _organizationManagementRepository = new(applicationDbContext, userManager);

        // Define the GetMyOrganizations endpoint to retrieve all organizations related to the currently signed-in user
        [HttpGet("/GetOrganizationsInfo")]
        public async Task<IActionResult> GetOrganizationsInfo()
        {
            // Call the repository method to get organizations related to the current user
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponceDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponceData = new List<string> { "User not found in token." }
                });
            }
            ApiResponceDTO result = await _organizationManagementRepository.GetOrganizationsInfo(userName);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if organizations were found
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if no organizations were found
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the CreateOrganization endpoint to create a new organization for the current user
        [HttpPost("/CreateOrganization")]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationDTO newOrganization)
        {
            // Call the repository method to create a new organization with the current user's ID
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponceDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponceData = new List<string> { "User not found in token." }
                });
            }
            ApiResponceDTO result = await _organizationManagementRepository.CreateOrganization(newOrganization, userName);
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
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the UpdateOrganization endpoint to update an existing organization for the current user
        [HttpPost("/UpdateOrganization")]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationDTO newOrganization)
        {
            // Call the repository method to update the organization with the current user's ID
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponceDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponceData = new List<string> { "User not found in token." }
                });
            }
            ApiResponceDTO result = await _organizationManagementRepository.UpdateOrganization(newOrganization, userName);
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
        [HttpPost("/DeleteOrganization")]
        public async Task<IActionResult> DeleteOrganization()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                // If the username is not found, return an unauthorized response
                return Unauthorized(new ApiResponceDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponceData = new List<string> { "User not found in token." }
                });
            }
            ApiResponceDTO result = await _organizationManagementRepository.DeactivateOrganization(userName);
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