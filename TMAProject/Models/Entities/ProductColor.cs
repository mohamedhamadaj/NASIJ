namespace TMAProject.Models.Entities
{
    public class ProductColor
    {
        public Guid Id { get; set; }


        // المنتج
        public Guid ProductId { get; set; }
        public Product Product { get; set; }


        // اللون
        public Guid ColorId { get; set; }
        public Color Color { get; set; }


        // صور اللون
        public ICollection<ProductColorImage> Images { get; set; } = new List<ProductColorImage>();


        // المقاسات والكميات
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
