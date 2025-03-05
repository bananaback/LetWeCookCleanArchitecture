using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IUserRepository
{
    Task<SiteUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SiteUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(SiteUser user, CancellationToken cancellationToken = default);
    Task RemoveAsync(SiteUser user, CancellationToken cancellationToken = default);
}