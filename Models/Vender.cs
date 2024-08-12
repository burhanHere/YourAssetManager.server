using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.Models
{
    public class Vender
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add a Vender Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please add a valid email.")]
        [EmailAddress]
        public string Email { get; set; }
        public string OfficeAddress { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}