using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.DTOs
{
    public class SignUpDTO
    {
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required, MaxLength(100)]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string requiredRole { get; set; }
    }
}