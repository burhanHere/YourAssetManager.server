using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string CategoryName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(1000)]
        public string RelevantInputFields { get; set; }

        [Required]
        public int CategoryOrganizationId { get; set; }

        [ForeignKey("CategoryOrganizationId")]
        public Organization Organization { get; set; }
    }
}
