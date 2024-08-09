using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YourAssetManager.Server.DTOs;

namespace YourAssetManager.Server.Models
{
    public class AssetSubCategories
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Category Name.")]
        public string SubCategoryName { get; set; }

        [ForeignKey("Catagories")]
        public int AssetCategoryId { get; set; }
        public AssetCategory AssetCategory { get; set; }
    }
}