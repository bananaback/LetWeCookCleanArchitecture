using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class SiteUserConfiguration : IEntityTypeConfiguration<SiteUser>
{
    public void Configure(EntityTypeBuilder<SiteUser> builder)
    {
        builder.Ignore(su => su.DomainEvents);

        builder.ToTable("site_users");

        builder.HasKey(su => su.Id);

        builder.Property(su => su.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(su => su.IsRemoved)
            .HasColumnName("is_removed")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(su => su.DateJoined)
            .HasColumnName("date_joined")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.HasOne(su => su.Profile)
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);


    }
}