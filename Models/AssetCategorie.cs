using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("Organization")]
        public int CatagoryOrganizationId { get; set; }

        public Organization Organization { get; set; }
    }
}