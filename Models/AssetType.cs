using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Asset Type Name.")]
        public string AssetTypeName { get; set; }
        public string Description { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}