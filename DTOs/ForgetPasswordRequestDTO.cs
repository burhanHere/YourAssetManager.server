using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.DTOs
{
    public class ForgetPasswordRequestDTO
    {
        [Required(ErrorMessage = "User Email is required")]
        public string Email { get; set; }
    }
}