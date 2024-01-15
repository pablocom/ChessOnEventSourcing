namespace ChessOnEventSourcing.Domain;

public interface IAggregateRoot<TId> where TId : notnull
{
    IReadOnlyList<DomainEvent<TId>> DomainEvents { get; }
    void ClearDomainEvents();
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId> where TId : IFormattable, notnull
{
    private readonly List<DomainEvent<TId>> _domainEvents = [];
    public IReadOnlyList<DomainEvent<TId>> DomainEvents => _domainEvents.ToArray().AsReadOnly();

    protected void AddDomainEvent(DomainEvent<TId> domainEvent) => _domainEvents.Add(domainEvent);
    protected void AddDomainEvents(params DomainEvent<TId>[] domainEvents) => _domainEvents.AddRange(domainEvents);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
