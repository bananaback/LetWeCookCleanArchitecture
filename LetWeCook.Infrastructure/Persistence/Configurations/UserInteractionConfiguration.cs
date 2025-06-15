using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class UserInteractionConfiguration : IEntityTypeConfiguration<UserInteraction>
{
    public void Configure(EntityTypeBuilder<UserInteraction> builder)
    {
        builder.ToTable("user_interactions");

        builder.HasKey(ui => ui.Id);

        builder.Property(ui => ui.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        builder.Property(ui => ui.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ui => ui.RecipeId)
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ui => ui.InteractionType)
            .HasColumnName("interaction_type")
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder.Property(ui => ui.EventValue)
            .HasColumnName("event_value")
            .HasColumnType("float")
            .IsRequired();

        builder.Property(ui => ui.InteractionDate)
            .HasColumnName("interaction_date")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
