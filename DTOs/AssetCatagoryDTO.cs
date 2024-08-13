using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.DTOs
{
    public class AssetCatagoryDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string RelaventInputFields { get; set; } // list special columns related to a specific catagory that will be input sepratedly when an asset is assigned to that catagory 
    }
}