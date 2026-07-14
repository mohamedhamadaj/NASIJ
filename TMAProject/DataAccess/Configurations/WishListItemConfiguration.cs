using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class WishListItemConfiguration : IEntityTypeConfiguration<WishListItem>
    {
        public void Configure(EntityTypeBuilder<WishListItem> builder)
        {
            builder.HasKey(w => w.Id);
            builder.HasOne(w => w.WishList)
                   .WithMany(wl => wl.WishListItems)
                   .HasForeignKey(w => w.WishListId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(w => w.Product)
                   .WithMany(p => p.WishListItems)
                   .HasForeignKey(w => w.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(w => new { w.WishListId, w.ProductId }).IsUnique();
        }
    }
}
