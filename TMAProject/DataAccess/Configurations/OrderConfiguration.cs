using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.NetworkInformation;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;

namespace TMAProject.DataAccess.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.OrderDate).IsRequired();
            builder.Property(o => o.ReceiverName).IsRequired().HasMaxLength(100);
            builder.Property(o => o.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(o => o.Address).IsRequired().HasMaxLength(200);
            builder.Property(o => o.City).IsRequired().HasMaxLength(100);
            builder.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(OrderStatus.Pending);
            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PaymentMethod.OnCashDelivery);
            builder.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PaymentStatus.Pending);

        }
    }
}
