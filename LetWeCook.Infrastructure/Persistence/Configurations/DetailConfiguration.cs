using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class DetailConfiguration : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.ToTable("details");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(d => d.Title)
            .HasColumnName("title")
            .HasColumnType("nvarchar(255)")
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasColumnType("nvarchar(1024)")
            .IsRequired();


        builder.HasMany(d => d.MediaUrls)
            .WithMany()
            .UsingEntity<DetailMediaUrl>(
                j => j.HasOne(dmu => dmu.MediaUrl)
                    .WithMany()
                    .HasForeignKey(dmu => dmu.MediaUrlId),
                j => j.HasOne(dmu => dmu.Detail)
                    .WithMany()
                    .HasForeignKey(dmu => dmu.DetailId),
                j =>
                {
                    j.ToTable("detail_media_urls");
                    j.HasKey(dmu => new { dmu.DetailId, dmu.MediaUrlId });
                    j.HasIndex(dmu => dmu.MediaUrlId).IsUnique();

                    j.Property(dmu => dmu.MediaUrlId)
                        .HasColumnName("media_url_id")
                        .HasColumnType("uniqueidentifier")
                        .IsRequired();

                    j.Property(dmu => dmu.DetailId)
                        .HasColumnName("detail_id")
                        .HasColumnType("uniqueidentifier")
                        .IsRequired();
                }
            );
    }
}
