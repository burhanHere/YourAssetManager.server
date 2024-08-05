using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class SignInModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}