namespace YourAssetManager.Server.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool ActiveUser { get; set; }
        public List<string> Roles { get; set; }
        public string ImagePath { get; set; }
    }
}