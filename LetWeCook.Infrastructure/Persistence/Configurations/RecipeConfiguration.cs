using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("recipes");

        builder.Ignore(r => r.DomainEvents);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(r => r.Servings)
            .IsRequired()
            .HasColumnName("servings")
            .HasColumnType("int");

        builder.Property(r => r.PrepareTime)
            .IsRequired()
            .HasColumnName("prepare_time")
            .HasColumnType("int");

        builder.Property(r => r.CookTime)
            .IsRequired()
            .HasColumnName("cook_time")
            .HasColumnType("int");

        builder.Ignore(r => r.TotalTime);

        builder.Property(r => r.DifficultyLevel)
            .IsRequired()
            .HasColumnName("difficulty_level")
            .HasConversion(
                v => v.ToString(),
                v => (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), v)
            );

        builder.Property(r => r.MealCategory)
            .IsRequired()
            .HasColumnName("meal_category")
            .HasConversion(
                v => v.ToString(),
                v => (MealCategory)Enum.Parse(typeof(MealCategory), v)
            );

        builder.HasMany(r => r.Tags)
            .WithMany(rt => rt.Recipes)
            .UsingEntity<RecipeTagging>(
                j => j
                    .HasOne(rt => rt.RecipeTag)
                    .WithMany()
                    .HasForeignKey("RecipeTagId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne(rt => rt.Recipe)
                    .WithMany()
                    .HasForeignKey("RecipeId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("recipe_tags_recipes");

                    j.HasKey("RecipeId", "RecipeTagId");

                    j.Property<Guid>("RecipeId")
                        .HasColumnName("recipe_id")
                        .HasColumnType("uniqueidentifier");

                    j.Property<Guid>("RecipeTagId")
                        .HasColumnName("recipe_tag_id")
                        .HasColumnType("uniqueidentifier");
                }
            );


        builder.HasOne(r => r.CoverMediaUrl)
            .WithOne()
            .HasForeignKey<Recipe>("CoverMediaUrlId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property<Guid>("CoverMediaUrlId")
            .HasColumnName("cover_media_url_id")
            .HasColumnType("uniqueidentifier");

        builder.HasOne(r => r.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property<Guid>("CreatedById")
            .HasColumnName("created_by_id")
            .HasColumnType("uniqueidentifier");

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasColumnType("datetime2");

        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at")
            .HasColumnType("datetime2");

        builder.Property(r => r.AverageRating)
            .IsRequired()
            .HasColumnName("average_rating")
            .HasColumnType("float");

        builder.Property(r => r.TotalRatings)
            .IsRequired()
            .HasColumnName("total_ratings")
            .HasColumnType("int");

        builder.Property(r => r.TotalViews)
            .IsRequired()
            .HasColumnName("total_views")
            .HasColumnType("int");

        builder.Property(r => r.IsVisible)
            .IsRequired()
            .HasColumnName("is_visible")
            .HasColumnType("bit");

        builder.Property(r => r.IsPreview)
            .IsRequired()
            .HasColumnName("is_preview")
            .HasColumnType("bit");

        builder.HasMany(i => i.RecipeDetails)
            .WithOne()
            .HasForeignKey("RecipeId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}