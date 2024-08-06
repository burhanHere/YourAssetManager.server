using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AssignedData { get; set; } = DateTime.Now;

        [ForeignKey("Assignee")]
        public string AssigneeId { get; set; }

        [ForeignKey("Assigner")]
        public string AssignerId { get; set; }

        [ForeignKey("Asset")]
        public int AssetId { get; set; }

        [ForeignKey("LogAction")]
        public int LogActionId { get; set; }


    }
}