using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.DTOs
{
    public class NewUserDTO
    {
        public string Roles { get; set; }
        [EmailAddress]
        public string email { get; set; }
    }
}