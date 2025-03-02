using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Application.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task DispatchEventsAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Process blocking handlers synchronously
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = serviceProvider.GetServices(handlerType);
            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                await (Task)method.Invoke(handler, new object[] { domainEvent, cancellationToken });
            }
        }

        // Dispatch non-blocking handlers in the background
        _ = Task.Run(async () =>
        {
            using var backgroundScope = _serviceScopeFactory.CreateScope();
            var scopedProvider = backgroundScope.ServiceProvider;

            var handlerTasks = new List<Task>();

            foreach (var domainEvent in domainEvents)
            {
                var handlerType = typeof(INonBlockingDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = scopedProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    // Call HandleAsync directly instead of wrapping in Task.Run
                    var method = handlerType.GetMethod("HandleAsync");
                    handlerTasks.Add((Task)method.Invoke(handler, new object[] { domainEvent, cancellationToken }));
                }
            }

            try
            {
                await Task.WhenAll(handlerTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing non-blocking handlers for events: {EventTypes}",
                    string.Join(", ", domainEvents.Select(e => e.GetType().Name)));
            }
        }, cancellationToken);
    }
}