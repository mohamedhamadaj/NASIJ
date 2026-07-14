namespace TMAProject.Models.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public int Rating { get; set; } = 0;
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
