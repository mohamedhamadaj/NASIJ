using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMAProject.Models.Entities;

namespace TMAProject.DataAccess.Configurations
{
    public class WishListConfiguration : IEntityTypeConfiguration<WishList>
    {
        public void Configure(EntityTypeBuilder<WishList> builder)
        {
            builder.HasKey(w => w.Id);
            builder.HasOne(w => w.User)
                   .WithOne(u=> u.WishList)
                   .HasForeignKey<WishList>(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
