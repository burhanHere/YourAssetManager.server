using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class AssetType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Asset Type Name.")]
        public string AssetTypeName { get; set; }

        public string Description { get; set; }
    }
}