using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class ProductColorImageConfiguration : IEntityTypeConfiguration<ProductColorImage>
    {
        void IEntityTypeConfiguration<ProductColorImage>.Configure(EntityTypeBuilder<ProductColorImage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.ImageUrl).IsRequired().HasMaxLength(255);
            builder.Property(p=>p.ImageUrl).IsRequired();
            builder.HasOne(i=>i.ProductColor)
                .WithMany(p=> p.Images)
                .HasForeignKey(i => i.ProductColorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
