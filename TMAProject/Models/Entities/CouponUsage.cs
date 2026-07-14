namespace TMAProject.Models.Entities
{
    public class CouponUsage
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!; 

        public Guid CouponId { get; set; }
        public Coupon Coupon { get; set; } = null!;
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
