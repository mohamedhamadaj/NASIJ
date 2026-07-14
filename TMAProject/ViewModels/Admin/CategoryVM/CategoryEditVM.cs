using TMAProject.Models.Enums;

namespace TMAProject.ViewModels.Admin.CategoryVM
{
    public class CategoryEditVM
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ExistingImageUrl { get; set; }
        public CategoryStatus Status { get; set; }
        public IFormFile? Image { get; set; }
    }
}
