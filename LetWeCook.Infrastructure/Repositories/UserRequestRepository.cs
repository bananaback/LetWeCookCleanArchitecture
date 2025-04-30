using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class UserRequestRepository : Repository<UserRequest>, IUserRequestRepository
{
    public UserRequestRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public async Task<UserRequest?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Id == requestId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.CreatedByUser.Id == userId)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<UserRequest?> GetPendingByNewReferenceIdAsync(Guid referenceId, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Where(r => r.NewReferenceId == referenceId && r.Status == UserRequestStatus.PENDING)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserRequest?> GetPendingByOldReferenceIdAsync(Guid referenceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.OldReferenceId == referenceId && r.Status == UserRequestStatus.PENDING)
            .FirstOrDefaultAsync(cancellationToken);
    }
}