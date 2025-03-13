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

        builder.Property(dp => dp.Color)
               .HasColumnName("color")
               .HasColumnType("nvarchar(10)")
               .IsRequired();

        builder.Property(dp => dp.Emoji)
               .HasColumnName("emoji")
               .HasColumnType("nvarchar(5)")
               .IsRequired();

        // Seed fixed values with descriptions, colors, and emojis
        builder.HasData(
            // Manual Tagging Preferences
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000001"), "None", "No specific dietary preference", "#808080", "❌"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000002"), "Vegetarian", "Excludes meat, includes dairy and eggs", "#4CAF50", "🥦"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000003"), "Vegan", "Excludes all animal products", "#8BC34A", "🌱"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000004"), "GlutenFree", "Excludes gluten-containing grains", "#FF9800", "🚫🌾"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000005"), "Pescatarian", "Excludes meat but allows fish", "#03A9F4", "🐟"),

            // Automatic Filtering Preferences
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000006"), "LowCalorie", "Focuses on low-calorie meals", "#FFEB3B", "🔥"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000007"), "HighProtein", "Emphasizes protein-rich foods", "#FF5722", "💪"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000008"), "LowCarb", "Limits carbohydrate intake (Keto-friendly)", "#9C27B0", "🥩"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000009"), "LowFat", "Focuses on reducing fat intake", "#2196F3", "🥗"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000010"), "LowSugar", "Reduces added sugar consumption", "#E91E63", "🚫🍭"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000011"), "HighFiber", "Emphasizes fiber-rich foods", "#673AB7", "🌾"),
            new DietaryPreference(new Guid("00000000-0000-0000-0000-000000000012"), "LowSodium", "Focuses on reducing salt intake", "#00BCD4", "🧂🚫")
        );
    }
}
