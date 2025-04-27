using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients");

        builder.Ignore(ri => ri.Id);

        builder.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier");

        builder.Property(ri => ri.IngredientId)
            .HasColumnName("ingredient_id")
            .HasColumnType("uniqueidentifier");

        builder.Property(ri => ri.Quantity)
            .IsRequired()
            .HasColumnName("quantity")
            .HasColumnType("float");

        builder.Property(ri => ri.Unit)
            .IsRequired()
            .HasColumnName("unit")
            .HasConversion(
                v => v.ToString(),
                v => (UnitEnum)Enum.Parse(typeof(UnitEnum), v)
            );

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(ri => ri.Ingredient)
            .WithMany()
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}