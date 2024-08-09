using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YourAssetManager.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore]
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();

        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }  // Foreign key to Organization (nullable to allow non-AssetManager roles)

        [JsonIgnore]
        public Organization Organization { get; set; }  // Navigation property to Organization
    }

}
