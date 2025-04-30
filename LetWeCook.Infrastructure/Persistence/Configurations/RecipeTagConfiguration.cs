using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
{
    public void Configure(EntityTypeBuilder<RecipeTag> builder)
    {
        builder.ToTable("recipe_tags");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(rt => rt.Name)
            .HasColumnName("name")
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder.HasData(
            new RecipeTag(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Vietnamese"),
            new RecipeTag(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Chinese"),
            new RecipeTag(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Mexican"),
            new RecipeTag(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Italian"),
            new RecipeTag(Guid.Parse("55555555-5555-5555-5555-555555555555"), "Indian"),
            new RecipeTag(Guid.Parse("66666666-6666-6666-6666-666666666666"), "Thai"),
            new RecipeTag(Guid.Parse("77777777-7777-7777-7777-777777777777"), "Japanese"),
            new RecipeTag(Guid.Parse("88888888-8888-8888-8888-888888888888"), "Korean"),
            new RecipeTag(Guid.Parse("99999999-9999-9999-9999-999999999999"), "American"),
            new RecipeTag(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "French"),
            new RecipeTag(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Mediterranean"),
            new RecipeTag(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Middle Eastern"),
            new RecipeTag(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Spanish"),
            new RecipeTag(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Greek"),
            new RecipeTag(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Turkish"),
            new RecipeTag(Guid.Parse("12345678-1234-1234-1234-123456789012"), "Brazilian"),
            new RecipeTag(Guid.Parse("23456789-2345-2345-2345-234567890123"), "Caribbean"),
            new RecipeTag(Guid.Parse("34567890-3456-3456-3456-345678901234"), "African"),
            new RecipeTag(Guid.Parse("45678901-4567-4567-4567-456789012345"), "German")
        );


    }
}