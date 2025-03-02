using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class DietaryPreferenceConfiguration : IEntityTypeConfiguration<DietaryPreference>
{

    public void Configure(EntityTypeBuilder<DietaryPreference> builder)
    {
        builder.ToTable("dietary_preferences");

        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(dp => dp.Name)
               .HasColumnName("name")
               .HasColumnType("nvarchar(50)")
               .IsRequired();

        // Seed fixed values with static Guids
        builder.HasData(
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000001"), "None"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000002"), "Vegetarian"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000003"), "Vegan"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000004"), "GlutenFree")
        );
    }
}