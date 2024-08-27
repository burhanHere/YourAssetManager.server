namespace YourAssetManager.Server.DTOs
{
    public class SearchDTO
    {
        string searchString { get; set; }
        List<string> Filters { get; set; }
    }
}