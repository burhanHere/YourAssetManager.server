namespace YourAssetManager.Server.DTOs
{
    public class AssetDTO
    {
        public int Id { get; set; }
        public string AssetName { get; set; }
        public string Description { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double PurchasePrice { get; set; }
        public string SerialNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? AssetIdentificationNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string? CatagoryReleventFeildsData { get; set; }
        public string OrganizationData { get; set; }
        public string AssetStatusData { get; set; }
        public string AssetCategoryData { get; set; }
        public string AssetTypeData { get; set; }
        public string VendorData { get; set; }
    }
}