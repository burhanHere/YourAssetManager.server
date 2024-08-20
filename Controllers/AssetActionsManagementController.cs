using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourAssetManager.Server.Controllers
{
    [Route("YourAssetManager.Server/[controlle]")]
    [ApiController]
    [Authorize(Roles = "OrganizationOwner")]
    public class AssetActionsManagementController : ControllerBase
    {

    }
}