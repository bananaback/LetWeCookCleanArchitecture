using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IUserRequestRepository : IRepository<UserRequest>
{
    Task<IEnumerable<UserRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserRequest?> GetPendingByOldReferenceIdAsync(Guid referenceId, CancellationToken cancellationToken = default);
    Task<UserRequest?> GetPendingByNewReferenceIdAsync(Guid referenceId, CancellationToken cancellationToken = default);
    Task<UserRequest?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
}