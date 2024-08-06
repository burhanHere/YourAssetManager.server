using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetMaintenance
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Maintenance Date.")]
        public DateTime MaintenanceDate { get; set; }

        public string Description { get; set; }

        [ForeignKey("Asset")]
        public int AssetId { get; set; }

        [ForeignKey("LogAction")]
        public int LogActionId { get; set; }

        public Asset Asset { get; set; }
        public LogAction LogAction { get; set; }
    }
}