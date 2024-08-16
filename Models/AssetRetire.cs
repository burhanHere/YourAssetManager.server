using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetRetire
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime RetiredOn { get; set; }

        [MaxLength(1000)]
        public string RetirementReason { get; set; }

        [Required]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }
    }
}
