namespace TMAProject.Models.Entities
{
    public class ProductColorImage
    {
        public Guid Id { get; set; }
        public Guid ProductColorId { get; set; }
        public ProductColor ProductColor { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
