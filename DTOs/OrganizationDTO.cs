using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourAssetManager.Server.DTOs
{
    public class OrganizationDTO
    {
        [Required(ErrorMessage = "Organization name is required")]
        public string OrganizationName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Organization domain is required")]
        public string OrganizationDomain { get; set; }
    }
}