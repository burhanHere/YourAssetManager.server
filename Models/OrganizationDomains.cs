using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace YourAssetManager.Server.Models
{
    public class OrganizationDomains
    {
        [Key]
        public int Id { get; set; }
        [RegularExpression(@"@[a-zA-Z0-9]+\.[a-zA-Z]{2,}", ErrorMessage = "Invalid domain")]
        public string OrganizationDomainString { get; set; }
        [Required]
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}