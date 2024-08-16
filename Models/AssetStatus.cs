using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class AssetStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string StatusName { get; set; }
    }
}
