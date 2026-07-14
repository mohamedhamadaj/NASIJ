namespace TMAProject.Models.Entities
{
    public class WishList
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<WishListItem> WishListItems { get; set; } = new List<WishListItem>();
    }
}
