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
}