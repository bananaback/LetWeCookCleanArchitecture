using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Persistence;

public class UserRepository : Repository<SiteUser>, IUserRepository
{
    public UserRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public Task<List<SiteUser>> GetAllWithProfileAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.Include(u => u.Profile)
            .ThenInclude(p => p.DietaryPreferences)
            .ToListAsync(cancellationToken);
    }

    public async Task<SiteUser?> GetWithProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(u => u.Profile).ThenInclude(p => p.DietaryPreferences).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}