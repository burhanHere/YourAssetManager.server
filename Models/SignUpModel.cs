using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class SignUpModel
    {
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required, MaxLength(100)]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}