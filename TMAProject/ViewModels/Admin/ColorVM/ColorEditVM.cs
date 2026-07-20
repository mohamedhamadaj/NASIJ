using System.ComponentModel.DataAnnotations;

namespace TMAProject.ViewModels.Admin.ColorVM
{
    public class ColorEditVM
    {
        public Guid ColorId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
