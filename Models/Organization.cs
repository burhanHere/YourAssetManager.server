using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YourAssetManager.Server.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string OrganizationName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public bool ActiveOrganization { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedDate { get; set; }

        //organizations also have domains in OrganizationDomains table
    }
}
