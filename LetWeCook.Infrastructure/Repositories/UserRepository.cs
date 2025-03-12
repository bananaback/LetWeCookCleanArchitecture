using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly LetWeCookDbContext _context;

    public UserRepository(LetWeCookDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SiteUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SiteUsers
            .FirstOrDefaultAsync(su => su.Id == id, cancellationToken);
    }

    public async Task AddAsync(SiteUser user, CancellationToken cancellationToken = default)
    {
        await _context.SiteUsers.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(SiteUser user, CancellationToken cancellationToken = default)
    {
        _context.SiteUsers.Update(user);
        return Task.CompletedTask; // EF Core tracks changes; no async operation needed here
    }

    public Task RemoveAsync(SiteUser user, CancellationToken cancellationToken = default)
    {
        _context.SiteUsers.Remove(user);
        return Task.CompletedTask; // Marking for removal; no async operation needed
    }

    public async Task<SiteUser?> GetWithProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SiteUsers.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}