using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly LetWeCookDbContext _context;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public UnitOfWork(LetWeCookDbContext context, IDomainEventDispatcher eventDispatcher)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        foreach (var entity in _context.ChangeTracker.Entries<AggregateRoot>())
        {
            entity.Entity.ClearDomainEvents();
        }

        int result = await _context.SaveChangesAsync(cancellationToken);

        await _eventDispatcher.DispatchEventsAsync(domainEvents, cancellationToken);

        return result;
    }
}