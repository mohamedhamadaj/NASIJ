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
            builder.Property(pv => pv.ProductColorId).IsRequired();
            builder.Property(pv => pv.ProductColorId).IsRequired();
            builder.Property(pv => pv.SizeId).IsRequired();
            builder.Property(pv => pv.Quantity).IsRequired();
            builder.HasOne(pv => pv.ProductColor)
                   .WithMany(c => c.Variants)
                   .HasForeignKey(pv => pv.ProductColorId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pv => pv.Size)
                   .WithMany(s => s.ProductVariants)
                   .HasForeignKey(pv => pv.SizeId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(pv => new { pv.ProductColorId,pv.SizeId }).IsUnique();
            builder.Property(pv => pv.Quantity)
                   .HasDefaultValue(0);

        }
    }
}
