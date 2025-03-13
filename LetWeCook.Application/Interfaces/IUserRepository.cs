using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;
public interface IUserRepository : IRepository<SiteUser>
{
    Task<SiteUser?> GetWithProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
}