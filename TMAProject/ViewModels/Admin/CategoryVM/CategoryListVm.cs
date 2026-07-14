using TMAProject.Models.Enums;

namespace TMAProject.ViewModels.Admin.CategoryVM
{
    public class CategoryListVm
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescription { get; set; }
        public string? CategoryImageUrl { get; set; }
        public int ProductCount { get; set; }
        public CategoryStatus Status { get; set; }
    }
}
