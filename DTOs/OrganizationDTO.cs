using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.DTOs
{
    public class OrganizationDTO
    {
        public string OrganizationName { get; set; }

        public string Description { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string OrganizationDomain { get; set; }
    }
}