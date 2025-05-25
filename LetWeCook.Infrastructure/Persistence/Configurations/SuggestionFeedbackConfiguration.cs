using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class SuggestionFeedbackConfiguration : IEntityTypeConfiguration<SuggestionFeedback>
{
    public void Configure(EntityTypeBuilder<SuggestionFeedback> builder)
    {
        builder.ToTable("suggestion_feedbacks");

        builder.HasKey(sf => sf.Id);

        builder.Property(sf => sf.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(sf => sf.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(sf => sf.RecipeId)
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(sf => sf.LikeOrDislike)
            .HasColumnName("like_or_dislike")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(sf => sf.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(sf => sf.User)
            .WithMany()
            .HasForeignKey(sf => sf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sf => sf.Recipe)
            .WithMany()
            .HasForeignKey(sf => sf.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}