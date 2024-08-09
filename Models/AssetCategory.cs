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
        public string RelaventInputFields { get; set; } // list special columns related to a specific catagory that will be input sepratedly when an asset is assigned to that catagory 

        [ForeignKey("Organization")]
        public int CatagoryOrganizationId { get; set; }

        public Organization Organization { get; set; }
    }
}