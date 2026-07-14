using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Description)
                .HasMaxLength(500);
            builder.Property(c => c.ImageUrl)
                .HasMaxLength(200);
            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>();
            builder.HasIndex(c=> c.Name).IsUnique();
        }
    }
}
