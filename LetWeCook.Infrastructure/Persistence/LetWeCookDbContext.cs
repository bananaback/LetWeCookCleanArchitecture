using LetWeCook.Application.DTOs.UserInteractions;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Persistence;

public class LetWeCookDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<SiteUser> SiteUsers { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<DietaryPreference> DietaryPreferences { get; set; } = null!;
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<RecipeRating> RecipeRatings { get; set; } = null!;
    public DbSet<Donation> Donations { get; set; } = null!;
    public DbSet<AggregatedInteractionDto> AggregatedInteractions { get; set; } = null!;


    public LetWeCookDbContext(DbContextOptions<LetWeCookDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AggregatedInteractionDto>().HasNoKey().ToView(null); // ToView(null) indicates it's not mapped to a specific view

        builder.ApplyConfigurationsFromAssembly(typeof(LetWeCookDbContext).Assembly);
    }
}