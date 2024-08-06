using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace YourAssetManager.Server.Models
{
    public class OrganizationOwner : IdentityUser
    {
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>(); // Fixed initialization
    }
}
