using TMAProject.Models.Enums;

namespace TMAProject.Models.Entities
{
    public class Coupon
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; } 
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public decimal MaximumDiscountAmount { get; set; }
        public CouponStatus CouponStatus { get; set; } = CouponStatus.Active;

        public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
    }
}
