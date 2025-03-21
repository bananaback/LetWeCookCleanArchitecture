using LetWeCook.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class DetailMediaUrlConfiguration : IEntityTypeConfiguration<DetailMediaUrl>
{
    public void Configure(EntityTypeBuilder<DetailMediaUrl> builder)
    {
        builder.ToTable("detail_media_urls");

    }
}