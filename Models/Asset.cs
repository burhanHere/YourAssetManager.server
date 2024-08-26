using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssetName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public double PurchasePrice { get; set; }

        [Required]
        [MaxLength(255)]
        public string SerialNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssetIdentificationNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public string Manufacturer { get; set; }

        [Required]
        [MaxLength(255)]
        public string Model { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public int AssetStatusId { get; set; }

        [ForeignKey("AssetStatusId")]
        public AssetStatus AssetStatus { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        public string? CatagoryReleventFeildsData { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }

        [Required]
        public int AssetCategoryId { get; set; }

        [ForeignKey("AssetCategoryId")]
        public AssetCategory AssetCategory { get; set; }

        [Required]
        public int AssetTypeId { get; set; }

        [ForeignKey("AssetTypeId")]
        public AssetType AssetType { get; set; }

        [Required]
        public int VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }
    }
}
