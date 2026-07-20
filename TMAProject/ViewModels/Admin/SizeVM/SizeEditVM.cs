using System.ComponentModel.DataAnnotations;

namespace TMAProject.ViewModels.Admin.SizeVM
{
    public class SizeEditVM
    {
        public Guid SizeId { get; set; }
        [Required]
        public string SizeName { get; set; } = string.Empty;
    }
}
