namespace YourAssetManager.Server.DTOs
{
    public class AssetAssignmentDTO
    {
        public string AssignedToId { get; set; }
        public int AssetId { get; set; }
        public string Notes { get; set; }
        public int RequestId { get; set; }
    }
}