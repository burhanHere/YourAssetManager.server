using Microsoft.AspNetCore.Identity;
using YourAssetManager.Server.Data;

namespace YourAssetManager.Server.Repositories
{
    public class AssetManagementRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager = userManager;



    }
}