using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers"); // Default Identity table

        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.SiteUser)
               .WithOne()
               .HasForeignKey<ApplicationUser>(a => a.SiteUserId)
               .OnDelete(DeleteBehavior.Cascade); // Deletes ApplicationUser with SiteUser
    }
}