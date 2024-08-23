using Microsoft.AspNetCore.Identity;

namespace YourAssetManager.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool ActiveUser { get; set; } = true;
    }
}