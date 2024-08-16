using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetReturn
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ReturnedDate { get; set; } = DateTime.Now;

        [MaxLength(1000)]
        public string ReturnCondition { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [Required]
        public int AssetAssignmentId { get; set; }

        [ForeignKey("AssetAssignmentId")]
        public AssetAssignment AssetAssignment { get; set; }
    }
}
