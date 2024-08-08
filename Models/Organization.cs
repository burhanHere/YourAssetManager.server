using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Organization Name.")]
        public string OrganizationName { get; set; }

        public string Description { get; set; }
        public bool ActiveOrganization { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; }

        [Required(ErrorMessage = "Please enter Organization Domain.")]
        public string OrganizationDomain { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<ApplicationUser> AssetManagers { get; set; } = new List<ApplicationUser>();  // Asset Managers in this organization

    }
}