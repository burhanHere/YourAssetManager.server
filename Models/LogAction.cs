using System.ComponentModel.DataAnnotations;

namespace YourAssetManager.Server.Models
{
    public class LogAction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please add Action Name.")]
        public string ActionName { get; set; }
    }
}