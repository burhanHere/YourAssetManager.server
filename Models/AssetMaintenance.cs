using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetMaintenance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }
    }
}
