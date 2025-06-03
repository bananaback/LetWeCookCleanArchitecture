using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class RecipeCollectionItemConfiguration : IEntityTypeConfiguration<RecipeCollectionItem>
{
    public void Configure(EntityTypeBuilder<RecipeCollectionItem> builder)
    {
        builder.ToTable("recipe_collection_items");

        builder.Ignore(rci => rci.Id); // Ignore the base Id property since we are using composite keys

        builder.HasKey(ri => new { ri.RecipeId, ri.CollectionId });

        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ri => ri.CollectionId)
            .HasColumnName("collection_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ri => ri.AddedAt)
            .HasColumnName("added_at")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasOne(ri => ri.Recipe)
            .WithMany()
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(ri => ri.Collection)
            .WithMany(c => c.Recipes)
            .HasForeignKey(ri => ri.CollectionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}