using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(1000)]
        public string OfficeAddress { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}
