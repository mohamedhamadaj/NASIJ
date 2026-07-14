using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.HasKey(cu => cu.Id);

            builder.HasOne(cu => cu.User)
                   .WithMany(u => u.CouponUsages)
                   .HasForeignKey(cu => cu.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cu => cu.Coupon)
                   .WithMany(c => c.CouponUsages)
                   .HasForeignKey(cu => cu.CouponId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cu => cu.Order)
                   .WithOne(o => o.CouponUsage)
                   .HasForeignKey<CouponUsage>(cu => cu.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(cu => new
            {
                cu.UserId,
                cu.CouponId
            })
            .IsUnique();

        }
    }
}