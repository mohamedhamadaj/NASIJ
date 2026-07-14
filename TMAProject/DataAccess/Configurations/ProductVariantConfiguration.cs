using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(pv => pv.Id);
            builder.Property(pv => pv.ProductId).IsRequired();
            builder.Property(pv => pv.ColorId).IsRequired();
            builder.Property(pv => pv.SizeId).IsRequired();
            builder.Property(pv => pv.Quantity).IsRequired();
            builder.HasOne(pv => pv.Product)
                   .WithMany(p => p.ProductVariants)
                   .HasForeignKey(pv => pv.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pv => pv.Color)
                   .WithMany(c => c.ProductVariants)
                   .HasForeignKey(pv => pv.ColorId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(pv => pv.Size)
                   .WithMany(s => s.ProductVariants)
                   .HasForeignKey(pv => pv.SizeId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(pv => new { pv.ProductId, pv.ColorId, pv.SizeId }).IsUnique();
            builder.Property(pv => pv.Quantity)
                   .HasDefaultValue(0);

        }
    }
}
