using TMAProject.Models.Enums;

namespace TMAProject.ViewModels.Admin.ProductVM
{
    public class ProductListVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? MainImage { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public int TotalStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductStatus Status { get; set; }
    }
}
