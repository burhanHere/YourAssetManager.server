using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    // [Authorize(Roles = "OrganizationOwner")]
    // [ApiController]
    // [Route("YourAssetManager.Server/{controller}")]
    // public class AssetManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : ControllerBase
    // {
    //     private readonly AssetCatagoryManagementRepository _assetCatagoryManagementRepository = new(userManager, applicationDbContext);

    // [HttpGet("/GetAllAssets")]
    // public async Task<IActionResult> GetAllAssets() { return Ok(); }

    // [HttpGet("/GetAssetById")]
    // public async Task<IActionResult> GetAssetById() { return Ok(); }

    // [HttpPost("/CreateAsset")]
    // public async Task<IActionResult> CreateAsset() { return Ok(); }

    // [HttpPut("/UpdateAsset")]
    // public async Task<IActionResult> UpdateAsset(int id) { return Ok(); }

    // [HttpDelete("/DeleteAsset")]
    // public async Task<IActionResult> DeleteAsset(int id) { return Ok(); }

    // [HttpPut("/UpdateAssetStatusById")]
    // public async Task<IActionResult> UpdateAssetStatus(int id) { return Ok(); }

    // [HttpGet("GetAssetStatistics")]
    // public async Task<IActionResult> GetAssetStatistics() { return Ok(); }

    // [HttpPost("/AssignAsset")]
    // public async Task<IActionResult> AssignAsset([FromBody] AssetRequestDTO request) { return Ok(); }

    // [HttpPost("/UnassignAsset")]
    // public async Task<IActionResult> UnassignAsset([FromBody] AssetRequestDTO request) { return Ok(); }

    // [HttpGet("/GetAssetsAssignedToUser/{userId}")]
    // public async Task<IActionResult> GetAssetsAssignedToUser(int userId) { return Ok(); }

    // [HttpGet("/GetAssetAssignmentDetails/{assetId}")]
    // public async Task<IActionResult> GetAssetAssignmentDetails(int assetId) { return Ok(); }

    // [HttpPut("/UpdateAssetAssignment")]
    // public async Task<IActionResult> UpdateAssetAssignment([FromBody] AssetRequestDTO request) { return Ok(); }
    // }
}