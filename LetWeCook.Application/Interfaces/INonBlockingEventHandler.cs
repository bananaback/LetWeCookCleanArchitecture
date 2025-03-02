using LetWeCook.Domain.Common;

namespace LetWeCook.Application.Interfaces;

public interface INonBlockingDomainEventHandler<in TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}