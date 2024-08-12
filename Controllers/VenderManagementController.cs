using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [Authorize(Roles = "OrganizationOwner")]
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class VenderManagementController(ApplicationDbContext applicationDbContext) : ControllerBase
    {
        private readonly VenderManagementRepository _venderManagementRepository = new(applicationDbContext);
    }
}