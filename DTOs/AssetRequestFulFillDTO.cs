namespace YourAssetManager.Server.DTOs
{
    public class AssetRequestFulFillDTO
    {
        public string AssignedToId { get; set; }
        public int AssetId { get; set; }
        public string Notes { get; set; }
        public int RequestId { get; set; }
    }
}