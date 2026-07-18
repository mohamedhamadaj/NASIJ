using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class ColorConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.HexCode)
                .IsRequired()
                .HasMaxLength(7);
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
