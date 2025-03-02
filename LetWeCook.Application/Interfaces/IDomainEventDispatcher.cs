using LetWeCook.Domain.Common;

namespace LetWeCook.Application.Interfaces;
public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken);
}
