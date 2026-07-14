using TMAProject.Models.Enums;

namespace TMAProject.Models.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public CategoryStatus Status { get; set; } = CategoryStatus.Active;
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
