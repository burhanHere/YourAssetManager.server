using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class AssetCategories
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Category Name.")]
        public string CategoryName { get; set; }

        public string Description { get; set; }
        public string Features { get; set; }
    }
}