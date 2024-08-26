using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class AssetRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RequestDescription { get; set; }
        [Required]
        public string RequestStatus { get; set; }
        [Required]
        public string RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public ApplicationUser Requester { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}