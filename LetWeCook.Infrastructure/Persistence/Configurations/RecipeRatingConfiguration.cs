using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class RecipeRatingConfiguration : IEntityTypeConfiguration<RecipeRating>
{
    public void Configure(EntityTypeBuilder<RecipeRating> builder)
    {
        builder.ToTable("recipe_ratings");

        builder.HasKey(rr => new { rr.RecipeId, rr.UserId });

        builder.Property(rr => rr.RecipeId)
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier");

        builder.Property(rr => rr.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uniqueidentifier");

        builder.HasOne(rr => rr.Recipe)
            .WithMany(r => r.Ratings)
            .HasForeignKey(rr => rr.RecipeId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(rr => rr.User)
            .WithMany()
            .HasForeignKey(rr => rr.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(rr => rr.Rating)
            .IsRequired()
            .HasColumnName("rating")
            .HasColumnType("int");

        builder.Property(rr => rr.Comment)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("comment")
            .HasColumnType("nvarchar(500)");

        builder.Property(rr => rr.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasColumnType("datetime2");


        builder.Property(rr => rr.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at")
            .HasColumnType("datetime2");
    }
}