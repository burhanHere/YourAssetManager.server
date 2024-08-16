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
    [Route("YourAssetManager.Server/{controller}")]
    [ApiController]
    [Authorize(Roles = "OrganizationOwner")]
    public class AssetTypeManagementController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly AssetTypeManagementRepository _assetTypeRepository = new(applicationDbContext, userManager);

        // Define the CreateAssetType endpoint to create a new asset type for the current user's organization
        [HttpPost("/CreateAssetType")]
        public async Task<IActionResult> CreateAssetType(AssetTypeDTO assetType)
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
            // Call the repository method to create the asset type with the current user's ID
            ApiResponseDTO result = await _assetTypeRepository.CreateAssetType(userId!, assetType);

            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset type was successfully created
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the GetAllAssetTypes endpoint to retrieve all asset types for the current user's organization
        [HttpGet("/GetAllAssetTypes")]
        public async Task<IActionResult> GetAllAssetTypes()
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
            // Call the repository method to get all asset types associated with the current user
            ApiResponseDTO result = await _assetTypeRepository.GetAllAssetTypes(userId!);

            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if asset types were successfully retrieved
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if no asset types were found
                return NotFound(result);
            }
            // Return a badrequest response 
            return BadRequest(result);
        }

        // Define the GetAssetTypeById endpoint to retrieve a specific asset type by its ID
        [HttpGet("/GetAssetTypeById")]
        public async Task<IActionResult> GetAssetTypeById(int assetTypeId)
        {
            // Call the repository method to get the asset type by its ID
            ApiResponseDTO result = await _assetTypeRepository.GetAssetTypeById(assetTypeId);

            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset type was successfully retrieved
                return Ok(result);
            }
            // Return a NotFound response if the asset type was not found
            return NotFound(result);
        }

        // Define the UpdateAssetType endpoint to update an existing asset type
        [HttpPut("/UpdateAssetType")]
        public async Task<IActionResult> UpdateAssetType(AssetTypeDTO assetType)
        {
            // Call the repository method to update the asset type
            ApiResponseDTO result = await _assetTypeRepository.UpdateAssetType(assetType);

            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset type was successfully updated
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the asset type to be updated was not found
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the DeleteAssetType endpoint to delete an existing asset type by its ID
        [HttpDelete("/DeleteAssetType")]
        public async Task<IActionResult> DeleteAssetType(int assetTypeId)
        {
            // Call the repository method to delete the asset type by its ID
            ApiResponseDTO result = await _assetTypeRepository.DeleteAssetType(assetTypeId);

            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset type was successfully deleted
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the asset type was not found
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
    }
}
