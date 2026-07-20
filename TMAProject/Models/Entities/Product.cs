using TMAProject.Models.Enums;

namespace TMAProject.Models.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal AverageRating { get; set; } = 0;
        public Category Category { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ProductStatus Status { get; set; } = ProductStatus.Active;

        public ICollection<ProductSubImage> ProductSubImages { get; set; } = new List<ProductSubImage>();
        public ICollection<Review> ProductReviews { get; set; } = new List<Review>();
        public ICollection<WishListItem> WishListItems { get; set; } = new List<WishListItem>();
        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    }

}
