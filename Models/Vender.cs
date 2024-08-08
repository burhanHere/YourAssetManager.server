using System.ComponentModel.DataAnnotations;

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
    }
}