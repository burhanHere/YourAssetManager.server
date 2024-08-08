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

        // Define the GetMyOrganizations endpoint to get organizatoin realted ti current user(SignedIn)
        [HttpGet("/GetMyOrganizations")]
        public async Task<IActionResult> GetMyOrganizations()
        {
            ApiResponceDTO result = await _organizationRepository.GetMyOrganizations(ClaimTypes.Name.ToString());
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok();
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpPost("/CreateOrganization")]
        public async Task<IActionResult> CreateOrganization(OrganizationDTO newOrganization)
        {
            ApiResponceDTO result = await _organizationRepository.CreateOrganization(newOrganization, ClaimTypes.Name);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpPost("/UpdateOrganization")]
        public async Task<IActionResult> UpdateOrganization(OrganizationDTO newOrganization)
        {
            ApiResponceDTO result = await _organizationRepository.UpdateOrganization(newOrganization, ClaimTypes.Name);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
        [HttpPost("/OrganizationOwnerDetails")]
        public async Task<IActionResult> OrganizationOwnerDetails()
        {
            var result = await _organizationRepository.OrganizationOwnerDetails(ClaimTypes.Name);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

    }
}