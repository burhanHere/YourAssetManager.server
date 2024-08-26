namespace YourAssetManager.Server.DTOs
{
    public class UserProfileUpdateDTO
    {
        public string UserName { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}