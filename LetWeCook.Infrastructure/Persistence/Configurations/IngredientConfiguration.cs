using LetWeCook.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");

        builder.Ignore(su => su.DomainEvents);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        builder.Property(i => i.Description)
            .HasColumnName("description")
            .HasColumnType("nvarchar(500)")
            .IsRequired();

        builder.HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(i => i.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasOne(i => i.CreatedByUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired(true);

        builder.Property(i => i.Calories)
            .HasColumnName("calories")
            .HasColumnType("float");

        builder.Property(i => i.Protein)
            .HasColumnName("protein")
            .HasColumnType("float");

        builder.Property(i => i.Carbohydrates)
            .HasColumnName("carbohydrates")
            .HasColumnType("float");

        builder.Property(i => i.Fat)
            .HasColumnName("fat")
            .HasColumnType("float");

        builder.Property(i => i.Sugar)
            .HasColumnName("sugar")
            .HasColumnType("float");

        builder.Property(i => i.Fiber)
            .HasColumnName("fiber")
            .HasColumnType("float");

        builder.Property(i => i.Sodium)
            .HasColumnName("sodium")
            .HasColumnType("float");

        builder.Property(i => i.IsVegetarian)
            .HasColumnName("is_vegetarian")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(i => i.IsVegan)
            .HasColumnName("is_vegan")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(i => i.IsGlutenFree)
            .HasColumnName("is_gluten_free")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(i => i.IsPescatarian)
            .HasColumnName("is_pescatarian")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(i => i.Visible)
            .HasColumnName("visible")
            .HasColumnType("bit")
            .IsRequired(true);

        builder.Property(i => i.IsPreview)
            .HasColumnName("is_preview")
            .HasColumnType("bit")
            .IsRequired(true)
            .HasDefaultValue(false);

        builder.HasOne(i => i.CoverImageUrl)
            .WithOne()
            .HasForeignKey<Ingredient>(i => i.CoverImageUrlId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.Property(i => i.CoverImageUrlId)
            .HasColumnName("cover_image_url_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired(true);

        builder.Property(i => i.ExpirationDays)
            .HasColumnName("expiration_days")
            .HasColumnType("float")
            .IsRequired(false);

        builder.HasMany(i => i.Details)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}