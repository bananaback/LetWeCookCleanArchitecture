using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class UserRequestConfiguration : IEntityTypeConfiguration<UserRequest>
{
    public void Configure(EntityTypeBuilder<UserRequest> builder)
    {
        builder.ToTable("user_requests");

        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ur => ur.Type)
            .HasColumnName("type")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(ur => ur.OldReferenceId)
            .HasColumnName("old_reference_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(ur => ur.NewReferenceId)
            .HasColumnName("new_reference_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ur => ur.ResponseMessage)
            .HasColumnName("response_message")
            .HasColumnType("nvarchar(1500)")
            .IsRequired(false);

        builder.Property(ur => ur.Status)
            .HasColumnName("status")
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne(ur => ur.CreatedByUser)
            .WithMany()
            .HasForeignKey(ur => ur.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.Property(ur => ur.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(ur => ur.CreatedByUserName)
            .HasColumnName("created_by_user_name")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        builder.Property(ur => ur.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ur => ur.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("datetime2")
            .IsRequired(false)
            .HasDefaultValueSql("NULL");


    }
}