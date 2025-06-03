using LetWeCook.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;


public class RecipeCollectionConfiguration : IEntityTypeConfiguration<RecipeCollection>
{
    public void Configure(EntityTypeBuilder<RecipeCollection> builder)
    {
        builder.Ignore(rc => rc.DomainEvents);

        builder.ToTable("recipe_collections");

        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier");

        builder.Property(rc => rc.Name)
            .HasColumnName("name")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        builder.Property(rc => rc.RecipeCount)
            .HasColumnName("recipe_count")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(rc => rc.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property<Guid>("created_by_id")
            .HasColumnName("created_by_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasOne(rc => rc.CreatedBy)
            .WithMany()
            .HasForeignKey("created_by_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}