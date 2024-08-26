namespace YourAssetManager.Server.DTOs
{
    public class UserProfileUpdateDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}