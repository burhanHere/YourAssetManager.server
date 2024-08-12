using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class AssetCatagoryManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : ControllerBase
    {
        private readonly AssetCatagoryManagementRepository _assetCatagoryManagementRepository = new(userManager, applicationDbContext);

        // Define the GetAllAssetCategories endpoint to retrieve all asset categories for the current user
        [HttpGet("/GetAllAssetCategories")]
        public async Task<ApiResponseDTO> GetAllAssetCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // If the username is not found, return an unauthorized response
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            // Call the repository method to get all asset categories for the current user
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAllAssetCategories(userId);
            // if (result.Status == StatusCodes.Status200OK)
            // {
            //     // Return an OK response if asset categories were found
            //     return Ok(result);
            // }
            // else if (result.Status == StatusCodes.Status404NotFound)
            // {
            //     // Return a NotFound response if no asset categories were found
            //     return NotFound(result);
            // }
            // // Return a BadRequest response for any other errors
            // return BadRequest(result);
            return result;
        }

        // Define the GetAssetCategoryById endpoint to retrieve an asset category by its ID
        [HttpGet("/GetAssetCategoryById")]
        public async Task<ApiResponseDTO> GetAssetCategoryById(int AssetCatagoryId)
        {
            // Call the repository method to get an asset category by its ID
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAssetCategoryById(AssetCatagoryId);
            // if (result.Status == StatusCodes.Status200OK)
            // {
            //     // Return an OK response if the asset category was found
            //     return Ok(result);
            // }
            // // Return a NotFound response if the asset category was not found
            // return NotFound(result);
            return result;
        }

        // Define the CreateAssetCategory endpoint to create a new asset category
        [HttpPost("/CreateAssetCategory")]
        public async Task<IActionResult> CreateAssetCategory([FromBody] AssetCatagoryDTO assetCatagoryDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(userId))
                {
                    // If the username is not found, return an unauthorized response
                    return Unauthorized(new ApiResponseDTO
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        ResponseData = new List<string> { "User not found in token." }
                    });
                }
            }

            // Call the repository method to create a new asset category for the current user
            var result = await _assetCatagoryManagementRepository.CreateAssetCategory(userId, assetCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset category was successfully created
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                // Return a MethodNotAllowed response if the action is not allowed
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the UpdateAssetCategory endpoint to update an existing asset category
        [HttpPut("/UpdateAssetCategory")]
        public async Task<IActionResult> UpdateAssetCategory(int AssetCatagoryId, [FromBody] AssetCatagoryDTO assetCatagoryDTO)
        {
            // Call the repository method to update an existing asset category
            ApiResponseDTO result = await _assetCatagoryManagementRepository.UpdateAssetCategory(AssetCatagoryId, assetCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset category was successfully updated
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the asset category was not found
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the DeleteAssetCategory endpoint to delete an asset category
        [HttpDelete("/DeleteAssetCategory")]
        public async Task<IActionResult> DeleteAssetCategory(int AssetCatagoryId)
        {
            // Call the repository method to delete an asset category
            ApiResponseDTO result = await _assetCatagoryManagementRepository.DeleteAssetCatagory(AssetCatagoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the asset category was successfully deleted
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the asset category was not found
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                // Return a MethodNotAllowed response if the action is not allowed
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the GetAllAssetSubCategoriesByAssetCaragoryId endpoint to retrieve all subcategories by asset category ID
        [HttpGet("/GetAllAssetSubCategoriesByAssetCaragoryId")]
        public async Task<ApiResponseDTO> GetAllAssetSubCategoriesByAssetCaragoryId(int AssetCatagoryId)
        {
            // Call the repository method to get all subcategories for a given asset category
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAllAssetSubCategoriesByAssetCaragoryId(AssetCatagoryId);
            // if (result.Status == StatusCodes.Status200OK)
            // {
            //     // Return an OK response if subcategories were found
            //     return Ok(result);
            // }
            // // Return a NotFound response if no subcategories were found
            // return NotFound(result);
            return result;
        }

        // Define the GetAssetSubCategoryById endpoint to retrieve a subcategory by its ID
        [HttpGet("/GetAssetSubCategoryById")]
        public async Task<IActionResult> GetAssetSubCategoryById(int AssetSubCategoryId)
        {
            // Call the repository method to get a subcategory by its ID
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAssetSubCategoryById(AssetSubCategoryId);
            // if (result.Status == StatusCodes.Status200OK)
            // {
            //     // Return an OK response if the subcategory was found
            //     return Ok(result);
            // }
            // // Return a NotFound response if the subcategory was not found
            // return NotFound(result);
            return result;
        }

        // Define the CreateAssetSubCategory endpoint to create a new asset subcategory
        [HttpGet("/CreateAssetSubCategory")]
        public async Task<IActionResult> CreateAssetSubCategory(int assetCategoryId, AssetSubCatagoryDTO assetSubCatagoryDTO)
        {
            // Call the repository method to create a new subcategory for a given asset category
            ApiResponseDTO result = await _assetCatagoryManagementRepository.CreateAssetSubCategory(assetCategoryId, assetSubCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the subcategory was successfully created
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the parent category or subcategory was not found
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the UpdateAssetSubCategory endpoint to update an existing asset subcategory
        [HttpGet("/UpdateAssetSubCategory")]
        public async Task<IActionResult> UpdateAssetSubCategory(int assetSubCategoryId, AssetSubCatagoryDTO assetSubCatagoryDTO)
        {
            // Call the repository method to update an existing asset subcategory
            ApiResponseDTO result = await _assetCatagoryManagementRepository.UpdateAssetSubCategory(assetSubCategoryId, assetSubCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the subcategory was successfully updated
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the subcategory was not found
                return NotFound(result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }

        // Define the DeleteAssetSubCategory endpoint to delete a subcategory
        [HttpDelete("/DeleteAssetSubCategory")]
        public async Task<IActionResult> DeleteAssetSubCategory(int assetSubCategoryId)
        {
            // Call the repository method to delete a subcategory
            ApiResponseDTO result = await _assetCatagoryManagementRepository.DeleteAssetSubCategory(assetSubCategoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response if the subcategory was successfully deleted
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response if the subcategory was not found
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status403Forbidden)
            {
                // Return a Forbidden response if access is denied
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            // Return a BadRequest response for any other errors
            return BadRequest(result);
        }
    }
}