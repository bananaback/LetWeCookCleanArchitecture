using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Events;

namespace LetWeCook.Infrastructure.Services.EventHandlers;

public class UserLoggedInNotificationHandler : INonBlockingDomainEventHandler<UserLoggedInEvent>
{
    public Task HandleAsync(UserLoggedInEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[NOTIFY] User {domainEvent.Email} logged in at {domainEvent.OccurredOn}");
        return Task.CompletedTask;
    }
}