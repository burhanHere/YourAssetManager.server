namespace YourAssetManager.Server.DTOs
{
    // All responses from any endpoint's respective service or repository function will return an object of this class in this project.
    public class ApiResponseDTO
    {
        public int Status { get; set; }
        public object? ResponseData { get; set; }
        public object? Errors { get; set; }
    }
}