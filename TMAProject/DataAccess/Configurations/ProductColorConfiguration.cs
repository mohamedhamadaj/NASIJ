using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
    {
        void IEntityTypeConfiguration<ProductColor>.Configure(EntityTypeBuilder<ProductColor> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(pc=> pc.Product)
                .WithMany(p=> p.ProductColors)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pc=>pc.Color)
                .WithMany(c=>c.ProductColors)
                .HasForeignKey(p=>p.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(pc => pc.Images)
                .WithOne(i => i.ProductColor)
                .HasForeignKey(i => i.ProductColorId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pc => pc.Variants)
                .WithOne(v => v.ProductColor)
                .HasForeignKey(v => v.ProductColorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pc=> new { pc.ProductId, pc.ColorId })
                .IsUnique();
        }
    }
}
