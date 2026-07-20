namespace TMAProject.Models.Entities
{
    public class ProductVariant
    {
        public Guid Id { get; set; }
        //public Guid ProductId { get; set; }
        //public Product Product { get; set; } = null!;
        public Guid ProductColorId { get; set; }
        public ProductColor ProductColor { get; set; } = null!;
        public Guid SizeId { get; set; }
        public Size Size { get; set; } = null!;
        public int Quantity { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
