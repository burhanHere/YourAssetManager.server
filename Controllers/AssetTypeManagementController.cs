
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "OrganizationOwner")]
    public class AssetTypeController(AssetTypeRepository assetTypeRepository) : ControllerBase
    {
        private readonly AssetTypeRepository _assetTypeRepository = assetTypeRepository;

        [HttpPost("/CreateAssetType")]
        public async Task<IActionResult> CreateAssetType(AssetTypeDTO assetType)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrEmpty())
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }

            ApiResponseDTO result = await _assetTypeRepository.CreateAssetType(userId!, assetType);

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

        [HttpGet("/GetAllAssetTypes")]
        public async Task<IActionResult> GetAllAssetTypes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrEmpty())
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                });
            }
            ApiResponseDTO result = await _assetTypeRepository.GetAllAssetTypes(userId!);

            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            return NotFound(result);
        }

        [HttpGet("/GetAssetTypeById")]
        public async Task<IActionResult> GetAssetTypeById(int assetTypeId)
        {
            ApiResponseDTO result = await _assetTypeRepository.GetAssetTypeById(assetTypeId);

            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost("/UpdateAssetType")]
        public async Task<IActionResult> UpdateAssetType(AssetTypeDTO assetType)
        {
            ApiResponseDTO result = await _assetTypeRepository.UpdateAssetType(assetType);

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

        [HttpPost("/DeleteAssetType")]
        public async Task<IActionResult> DeleteAssetType(int assetTypeId)
        {
            ApiResponseDTO result = await _assetTypeRepository.DeleteAssetType(assetTypeId);

            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status405MethodNotAllowed)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, result);
            }
            return BadRequest(result);
        }
    }
}