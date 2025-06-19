using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class DonationRepository : Repository<Donation>, IDonationRepository
{
    public DonationRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public Task<List<Donation>> GetCompletedDonationsByRecipeId(Guid recipeId, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(d => d.Donator)
                .ThenInclude(d => d.Profile)
            .Include(d => d.Author)
                .ThenInclude(a => a.Profile)
            .Include(d => d.Recipe)
                .ThenInclude(r => r.CoverMediaUrl)
            .Where(d => d.RecipeId == recipeId && d.Status == "Completed")
            .ToListAsync(cancellationToken);
    }

    public async Task<Donation?> GetDonationDetailsById(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(d => d.Donator)
                .ThenInclude(d => d.Profile)
            .Include(d => d.Author)
                .ThenInclude(a => a.Profile)
            .Include(d => d.Recipe)
                .ThenInclude(r => r.CoverMediaUrl)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<int> GetRecipeTotalDonationAmount(Guid recipeId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(d => d.RecipeId == recipeId && d.Status == "Completed")
            .SumAsync(d => d.Amount, cancellationToken)
            .ContinueWith(t => (int)t.Result, cancellationToken);
    }

    public Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        return _dbSet
            .CountAsync(cancellationToken);
    }

    public Task<Donation?> GetWithRecipeDonatorAndAuthorAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(d => d.Recipe)
            .Include(d => d.Author)
            .Include(d => d.Donator)
                .ThenInclude(d => d.Profile)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}