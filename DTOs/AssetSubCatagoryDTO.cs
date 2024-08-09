using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.DTOs
{
    public class AssetSubCatagoryDTO
    {
        [Required(ErrorMessage = "SubCategory Name is required.")]
        public string SubCategoryName { get; set; }
    }
}