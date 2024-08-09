using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using YourAssetManager.server.Data.Migrations;

namespace YourAssetManager.Server.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Please add Asset Name.")]
        public string AssetName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please add Asset status.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Please add Asset Purchase Date.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Please add Asset Purchase Price.")]
        public double PurchasePrice { get; set; }

        [Required(ErrorMessage = "Please add Asset Serial Number.")]
        public string SerialNumber { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; }
        public DateTime LastServiced { get; set; }
        public string Problems { get; set; }

        [Required(ErrorMessage = "Please add Asset Identification Number.")]
        public string AssetIdentificationNumber { get; set; }

        [Required(ErrorMessage = "Please add Asset Manufacturer.")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "Please add Asset Model.")]
        public string Model { get; set; }

        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        [ForeignKey("AssetCategory")]
        public int AssetCategoryId { get; set; }

        [ForeignKey("AssetSubCategory")]
        public int AssetSubCategoryId { get; set; }

        [ForeignKey("AssetType")]
        public int AssetTypeId { get; set; }

        [ForeignKey("Vender")]
        public int VenderId { get; set; }

        public Organization Organization { get; set; }
        public AssetCategory AssetCategory { get; set; }
        public AssetSubCategory AssetSubCategory { get; set; }
        public AssetType AssetType { get; set; }
        public Vender Vender { get; set; }
    }
}