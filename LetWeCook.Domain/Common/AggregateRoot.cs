namespace LetWeCook.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = new();

    protected AggregateRoot() { } // For EF Core

    protected AggregateRoot(Guid id) : base(id) { }

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}