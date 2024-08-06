using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace YourAssetManager.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }
}
