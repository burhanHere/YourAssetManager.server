using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Controllers
{
    [Route("YourAssetManager.Server /{controller}")]
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    public class OrganizationController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly OrganizationRepository _organizationRepository = new(applicationDbContext, userManager);

        // Define the GetMyOrganizations endpoint to retrieve all organizations related to the currently signed-in user
        [HttpGet("/GetMyOrganizations")]
        public async Task<IActionResult> GetMyOrganizations()
        {
            // Call the repository method to get organizations related to the current user
            ApiResponceDTO result = await _organizationRepository.GetMyOrganizations(ClaimTypes.Name.ToString());
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if organizations were found
                return Ok();
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
            ApiResponceDTO result = await _organizationRepository.CreateOrganization(newOrganization, ClaimTypes.Name);
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
            ApiResponceDTO result = await _organizationRepository.UpdateOrganization(newOrganization, ClaimTypes.Name);
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

        // Define the OrganizationOwnerDetails endpoint to get details about the organization owner (the current user)
        [HttpPost("/OrganizationOwnerDetails")]
        public async Task<IActionResult> OrganizationOwnerDetails()
        {
            // Call the repository method to get details of the organization owner
            var result = await _organizationRepository.OrganizationOwnerDetails(ClaimTypes.Name);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response with the owner details if found
                return Ok(result);
            }
            // Return a NotFound response if the owner details were not found
            return NotFound(result);
        }
    }
}