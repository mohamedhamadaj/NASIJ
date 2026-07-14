namespace TMAProject.Models.Entities
{
    public class WishListItem
    {
        public Guid Id { get; set; }
        public Guid WishListId { get; set; }
        public WishList WishList { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
