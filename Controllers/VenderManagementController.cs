using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class VenderManagementController(ApplicationDbContext applicationDbContext) : ControllerBase
    {
        private readonly VenderManagementRepository _venderManagementRepository = new(applicationDbContext);

        [HttpPost("/CreateVender")]
        public async Task<IActionResult> CreateVender(VenderDTO venderDTO)
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
            ApiResponseDTO result = await _venderManagementRepository.CreateVender(userId!, venderDTO);
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

        [HttpGet("/GetAllVenders")]
        public async Task<ApiResponseDTO> GetAllVenders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrEmpty())
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string> { "User not found in token." }
                };
            }
            ApiResponseDTO result = await _venderManagementRepository.GetAllVenders(userId!);
            return result;
        }

        [HttpPost("/UpdateVender")]
        public async Task<IActionResult> UpdateVender(VenderDTO venderUpdate)
        {
            ApiResponseDTO result = await _venderManagementRepository.UpdateVender(venderUpdate);
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

        [HttpDelete("/DeleteVender")]
        public async Task<IActionResult> DeleteVender(int venderId)
        {
            ApiResponseDTO result = await _venderManagementRepository.DeleteVender(venderId);

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

        [HttpGet("/GetVenderById")]
        public async Task<ApiResponseDTO> GetVenderById(int venderId)
        {
            ApiResponseDTO result = await _venderManagementRepository.GetVenderById(venderId);
            return result;
        }
    }
}