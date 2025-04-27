using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class IngredientDetailConfiguration : IEntityTypeConfiguration<IngredientDetail>
{
    public void Configure(EntityTypeBuilder<IngredientDetail> builder)
    {
        builder.ToTable("ingredient_details");

        builder.HasKey(ig => ig.Id);

        builder.Property(ig => ig.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property<Guid>("IngredientId")
            .HasColumnName("ingredient_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ig => ig.Order)
            .HasColumnName("order")
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne(id => id.Detail)
            .WithOne()
            .HasForeignKey<IngredientDetail>("DetailId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.Property<Guid>("DetailId")
            .HasColumnName("detail_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired(true);
    }
}