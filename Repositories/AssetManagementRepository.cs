using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;

namespace YourAssetManager.Server.Repositories
{
    public class AssetManagementRepository(ApplicationDbContext applicationDbContext)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        internal async Task<ApiResponseDTO> GetAllAssets(string userId)
        {
            var organization = await _applicationDbContext.Organizations.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);
            // var assets = _applicationDbContext.Assets.;
            return new ApiResponseDTO();
        }

    }
}