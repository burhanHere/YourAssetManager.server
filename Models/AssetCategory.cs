using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Category Name.")]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        [ForeignKey("Organization")]
        public int CatagoryOrganizationId { get; set; }

        public Organization Organization { get; set; }
    }
}