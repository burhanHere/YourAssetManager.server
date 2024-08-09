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

        [HttpGet("/GetAllAssetCategories")]
        public async Task<IActionResult> GetAllAssetCategories()
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
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAllAssetCategories(userId);
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

        [HttpGet("/GetAssetCategoryById")]
        public async Task<IActionResult> GetAssetCategoryById([FromBody] int AssetCatagoryId)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAssetCategoryById(AssetCatagoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

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
            var result = await _assetCatagoryManagementRepository.CreateAssetCategory(userId, assetCatagoryDTO);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            return BadRequest(result);
        }

        [HttpPut("/UpdateAssetCategory")]
        public async Task<IActionResult> UpdateAssetCategory([FromBody] int AssetCatagoryId, [FromBody] AssetCatagoryDTO assetCatagoryDTO)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.UpdateAssetCategory(AssetCatagoryId, assetCatagoryDTO);
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

        [HttpDelete("/DeleteAssetCategory")]
        public async Task<IActionResult> DeleteAssetCategory(int AssetCatagoryId)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.DeleteAssetCatagory(AssetCatagoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status403Forbidden)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return BadRequest(result);
        }
        [HttpGet("/GetAllAssetSubCategoriesByAssetCaragoryId")]
        public async Task<IActionResult> GetAllAssetSubCategoriesByAssetCaragoryId(int AssetCatagoryId)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAllAssetSubCategoriesByAssetCaragoryId(AssetCatagoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
        [HttpGet("/GetAssetSubCategoryById")]
        public async Task<IActionResult> GetAssetSubCategoryById(int AssetSubCategoryId)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.GetAssetSubCategoryById(AssetSubCategoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
        [HttpGet("/CreateAssetSubCategory")]
        public async Task<IActionResult> CreateAssetSubCategory(int assetCategoryId, AssetSubCatagoryDTO assetSubCatagoryDTO)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.CreateAssetSubCategory(assetCategoryId, assetSubCatagoryDTO);
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

        [HttpGet("/UpdateAssetSubCategory")]
        public async Task<IActionResult> UpdateAssetSubCategory(int assetSubCategoryId, AssetSubCatagoryDTO assetSubCatagoryDTO)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.UpdateAssetSubCategory(assetSubCategoryId, assetSubCatagoryDTO);
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
        [HttpDelete("/DeleteAssetSubCategory")]
        public async Task<IActionResult> DeleteAssetSubCategory(int assetSubCategoryId)
        {
            ApiResponseDTO result = await _assetCatagoryManagementRepository.DeleteAssetSubCategory(assetSubCategoryId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status403Forbidden)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return BadRequest(result);
        }
    }
}