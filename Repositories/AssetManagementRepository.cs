using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    public class AssetManagementRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        UserManager<IdentityUser> _userManager = userManager;

        internal async Task<ApiResponseDTO> GetAllAssets(string userId)
        {
            var organization = await _userManager.FindByIdAsync(userId);
            // var assets = _applicationDbContext.Assets.;
            return new ApiResponseDTO();
        }

    }
}