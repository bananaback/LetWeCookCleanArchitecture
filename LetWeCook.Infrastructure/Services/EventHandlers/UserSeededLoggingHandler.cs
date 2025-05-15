using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Events;

namespace LetWeCook.Infrastructure.Services.EventHandlers;

public class UserSeededLoggingHandler : INonBlockingDomainEventHandler<UserSeededEvent>
{
    public Task HandleAsync(UserSeededEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[NOTIFY] User {domainEvent.Username} seeded with email {domainEvent.Email} at {domainEvent.OccurredOn}");
        return Task.CompletedTask;
    }
}