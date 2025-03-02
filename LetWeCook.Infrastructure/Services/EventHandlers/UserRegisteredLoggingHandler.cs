using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Events;

namespace LetWeCook.Infrastructure.Services.EventHandlers;

public class UserRegisteredLoggingHandler : IDomainEventHandler<UserRegisteredEvent>
{
    public Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[LOG] User {domainEvent.Email} (ID: {domainEvent.UserId}) registered at {domainEvent.OccurredOn}");
        return Task.CompletedTask;
    }
}