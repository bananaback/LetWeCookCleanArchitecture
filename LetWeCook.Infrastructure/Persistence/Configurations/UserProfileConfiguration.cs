using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(us => us.Id);

        builder.Property(us => us.Id)
            .HasColumnName("id");

        builder.OwnsOne(up => up.Address, address =>
        {
            address.Property(a => a.HouseNumber)
                .HasColumnName("house_number")
                .HasColumnType("nvarchar(10)")
                .IsRequired();

            address.Property(a => a.Street)
                .HasColumnName("street")
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            address.Property(a => a.Ward)
                .HasColumnName("ward")
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            address.Property(a => a.District)
                .HasColumnName("district")
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            address.Property(a => a.ProvinceOrCity)
                .HasColumnName("province_or_city")
                .HasColumnType("nvarchar(50)")
                .IsRequired();
        });

        builder.Property(up => up.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(up => up.Email)
            .HasColumnName("email")
            .HasColumnType("nvarchar(255)")
            .IsRequired();

        builder.Property(up => up.Gender)
            .HasConversion<int>()
            .HasColumnName("gender")
            .IsRequired();

        builder.OwnsOne(us => us.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnType("nvarchar(50)")
                .HasColumnName("first_name")
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnType("nvarchar(50)")
                .HasColumnName("last_name")
                .IsRequired();
        });

        builder.Property(up => up.Bio)
            .HasColumnName("bio")
            .HasColumnType("nvarchar(500)") // Allowing up to 500 characters for flexibility
            .IsRequired(false);

        builder.Property(up => up.Facebook)
                    .HasColumnName("facebook_url")
                    .HasColumnType("nvarchar(255)")
                    .IsRequired(false);

        builder.Property(up => up.Instagram)
            .HasColumnName("instagram_url")
            .HasColumnType("nvarchar(255)")
            .IsRequired(false);


        builder.Property(up => up.PhoneNumber)
            .HasColumnType("nvarchar(15)")
            .HasColumnName("phone_number")
            .IsRequired(false);

        builder.Property(up => up.ProfilePic)
            .HasColumnName("profile_picture_url")
            .HasColumnType("nvarchar(255)")
            .IsRequired(false);

        builder.HasMany(up => up.DietaryPreferences)
               .WithMany()
               .UsingEntity<UserDietaryPreference>(
                   j => j.HasOne(udp => udp.DietaryPreference)
                         .WithMany()
                         .HasForeignKey(udp => udp.DietaryPreferenceId),
                   j => j.HasOne(udp => udp.UserProfile)
                         .WithMany()
                         .HasForeignKey(udp => udp.UserProfileId),
                   j =>
                   {
                       j.ToTable("user_dietary_preferences");

                       j.HasKey(udp => new { udp.UserProfileId, udp.DietaryPreferenceId });

                       j.Property(udp => udp.UserProfileId)
                        .HasColumnName("user_profile_id")
                        .HasColumnType("uniqueidentifier")
                        .IsRequired();

                       j.Property(udp => udp.DietaryPreferenceId)
                        .HasColumnName("dietary_preference_id")
                        .HasColumnType("uniqueidentifier")
                        .IsRequired();
                   });

        builder.Property(up => up.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
    }
}
