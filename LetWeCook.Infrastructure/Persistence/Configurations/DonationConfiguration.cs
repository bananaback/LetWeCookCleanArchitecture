using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetWeCook.Infrastructure.Persistence.Configurations;

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("donations");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(d => d.TransactionId)
            .HasColumnName("transaction_id")
            .HasColumnType("nvarchar(255)")
            .IsRequired();

        builder.Property(d => d.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(d => d.Currency)
            .HasColumnName("currency")
            .HasColumnType("nvarchar(3)")
            .IsRequired();

        builder.Property(d => d.DonateMessage)
            .HasColumnName("donate_message")
            .HasColumnType("nvarchar(500)")
            .IsRequired(false);

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder.Property(d => d.ApprovalUrl)
            .HasColumnName("approval_url")
            .HasColumnType("nvarchar(500)")
            .IsRequired(false);

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(d => d.Recipe)
            .WithMany(r => r.Donations)
            .HasForeignKey(d => d.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Donator)
            .WithMany(u => u.DonationsMade)
            .HasForeignKey(d => d.DonatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Author)
            .WithMany(u => u.DonationsReceived)
            .HasForeignKey(d => d.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.RecipeId)
            .HasColumnName("recipe_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(d => d.DonatorId)
            .HasColumnName("donator_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(d => d.AuthorId)
            .HasColumnName("author_id")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
    }
}