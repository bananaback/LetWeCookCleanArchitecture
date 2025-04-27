using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class RecipeDetailConfiguration : IEntityTypeConfiguration<RecipeDetail>
{
    public void Configure(EntityTypeBuilder<RecipeDetail> builder)
    {
        builder.ToTable("recipe_details");

        builder.HasKey(id => id.Id);

        builder.Property(id => id.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(rp => rp.Order)
            .HasColumnName("order")
            .HasColumnType("int")
            .IsRequired();

        builder.Property<Guid>("RecipeId")
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasOne(id => id.Detail)
            .WithOne()
            .HasForeignKey<RecipeDetail>("DetailId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.Property<Guid>("DetailId")
            .HasColumnName("detail_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired(true);
    }

}