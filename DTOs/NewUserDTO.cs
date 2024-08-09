using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.DTOs
{
    public class NewUserDTO
    {
        [Required(ErrorMessage = "Role name is required")]
        public string Roles { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "User Email is required")]
        public string email { get; set; }
    }
}