using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;

namespace YourAssetManager.Server.Repositories
{
    public class VenderManagementRepository(ApplicationDbContext applicationDbContext)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public async Task<ApiResponseDTO> CreateVender(VenderDTO venderDTO)
        {

        }
    }
}