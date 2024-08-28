using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace YourAssetManager.Server.Models
{
    public class AssetAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [MaxLength(1000)]
        public string Notes { get; set; }

        [Required]
        public string AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public ApplicationUser AssignedTo { get; set; }

        [Required]
        public string AssignedById { get; set; }

        [ForeignKey("AssignedById")]
        public ApplicationUser AssignedBy { get; set; }

        [Required]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }
    }
}
