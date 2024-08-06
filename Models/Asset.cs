using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace YourAssetManager.Server.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }
        [Required(ErrorMessage = "Plase add Asset Name.")]
        public string AssetName { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Plase add Asset status.")]
        public string status { get; set; }
        [Required(ErrorMessage = "Plase add Asset Purchase Date.")]
        public DateTime PurchaseDate { get; set; }
        [Required(ErrorMessage = "Plase add Asset Purchase Price.")]
        public double PurchasePrice { get; set; }
        [Required(ErrorMessage = "Plase add Asset Serial Number.")]
        public string SerialNumber { get; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; }
        public DateTime LastServiced { get; set; }
        public string Problems { get; set; }
        [Required(ErrorMessage = "Plase add Asset Asset Identification Number.")]
        public string AssetIdentificationNumber { get; set; }
        [Required(ErrorMessage = "Plase add Asset Manufacturer.")]
        public string Manufacturer { get; set; }
        [Required(ErrorMessage = "Plase add Asset Model.")]
        public string Model { get; set; }
        public virtual Organization Organization { get; set; }
        // [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

    }
}