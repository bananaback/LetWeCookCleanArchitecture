using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class MediaUrlConfiguration : IEntityTypeConfiguration<MediaUrl>
{
    public void Configure(EntityTypeBuilder<MediaUrl> builder)
    {
        builder.ToTable("media_urls");

        builder.HasKey(mu => mu.Id);

        builder.Property(mu => mu.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(mu => mu.MediaType)
            .HasColumnName("media_type")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(mu => mu.Url)
            .HasColumnName("url")
            .HasColumnType("nvarchar(255)")
            .IsRequired();
    }
}