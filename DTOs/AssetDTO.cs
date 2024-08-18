namespace YourAssetManager.Server.DTOs
{
    public class AssetDTO
    {
        public string AssetName { get; set; }
        public string Description { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double PurchasePrice { get; set; }
        public string SerialNumber { get; set; }
        public string AssetIdentificationNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string CatagoryReleventFeildsData { get; set; }
        public int OrganizationId { get; set; }
        public int AssetStatusId { get; set; }
        public int AssetCategoryId { get; set; }
        public int AssetTypeId { get; set; }
        public int VendorId { get; set; }
    }
}