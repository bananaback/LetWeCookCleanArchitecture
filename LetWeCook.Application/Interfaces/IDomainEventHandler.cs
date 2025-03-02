using LetWeCook.Domain.Common;

namespace LetWeCook.Application.Interfaces;

public interface IDomainEventHandler<T> where T : DomainEvent
{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken);
}