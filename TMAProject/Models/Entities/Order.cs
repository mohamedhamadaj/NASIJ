using TMAProject.Models.Enums;

namespace TMAProject.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; } = 0;
        public CouponUsage? CouponUsage { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.OnCashDelivery;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
