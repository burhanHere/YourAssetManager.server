using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class ResetPassowrdModel
    {
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm New Password does not match.")]
        public string? ConfirmedNewPassword { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }
    }
}