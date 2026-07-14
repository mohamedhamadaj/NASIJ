using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class ProductSubImageConfiguration : IEntityTypeConfiguration<ProductSubImage>
    {
        public void Configure(EntityTypeBuilder<ProductSubImage> builder)
        {
            builder.HasKey(ps => new { ps.Id });
            builder.HasOne(ps => ps.Product)
                   .WithMany(p => p.ProductSubImages)
                   .HasForeignKey(ps => ps.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(ps => ps.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(255);
        }
    }
}
