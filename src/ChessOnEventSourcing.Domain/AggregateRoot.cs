namespace ChessOnEventSourcing.Domain;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.ToArray().AsReadOnly();
    
    public int Version { get; protected set; }

    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    protected void AddDomainEvents(params DomainEvent[] domainEvents) => _domainEvents.AddRange(domainEvents);
    public void ClearDomainEvents() => _domainEvents.Clear();
    
    public abstract void Apply(DomainEvent @event);
}
