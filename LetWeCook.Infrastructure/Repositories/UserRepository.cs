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

    public async Task<SiteUser?> GetWithProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}