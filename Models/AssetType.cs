using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssetTypeName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}
