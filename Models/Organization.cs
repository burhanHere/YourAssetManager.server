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
        public string Descriiption { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; }
        [Required(ErrorMessage = "Please enter organization Domain")]
        public string OrganizationDomain { get; set; }

        [ForeignKey("OrganizationOwner")]
        public string OrganizationOwnerId { get; set; } // Fixed foreign key property name

        public OrganizationOwner OrganizationOwner { get; set; } // Changed to OrganizationOwner
    }
}