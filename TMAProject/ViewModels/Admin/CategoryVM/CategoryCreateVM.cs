using System.ComponentModel.DataAnnotations;
using TMAProject.Models.Enums;

namespace TMAProject.ViewModels.Admin.CategoryVM
{
    public class CategoryCreateVM
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(300)]
        public string? Description { get; set; } 
        public IFormFile? Image { get; set; }
        public CategoryStatus Status { get; set; } = CategoryStatus.Active;

    }
}
