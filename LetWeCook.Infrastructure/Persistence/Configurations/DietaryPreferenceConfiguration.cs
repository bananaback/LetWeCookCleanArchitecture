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

        builder.Property(dp => dp.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(100)")
                .IsRequired();

        // Seed fixed values with descriptions
        builder.HasData(
            // Manual Tagging Preferences
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000001"), "None", "No specific dietary preference"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000002"), "Vegetarian", "Excludes meat, includes dairy and eggs"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000003"), "Vegan", "Excludes all animal products"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000004"), "GlutenFree", "Excludes gluten-containing grains"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000005"), "Pescatarian", "Excludes meat but allows fish"),

            // Automatic Filtering Preferences
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000006"), "LowCalorie", "Focuses on low-calorie meals"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000007"), "HighProtein", "Emphasizes protein-rich foods"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000008"), "LowCarb", "Limits carbohydrate intake (Keto-friendly)"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000009"), "LowFat", "Focuses on reducing fat intake"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000010"), "LowSugar", "Reduces added sugar consumption"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000011"), "HighFiber", "Emphasizes fiber-rich foods"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000012"), "LowSodium", "Focuses on reducing salt intake")
        );
    }
}
