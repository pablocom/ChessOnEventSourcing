namespace ChessOnEventSourcing.Domain;

public abstract class AggregateRoot : Entity
{
    public int Version { get; protected set; }
    
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.ToArray().AsReadOnly();
    
    public void ClearDomainEvents() => _domainEvents.Clear();
    public abstract void Apply(DomainEvent @event);

    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    protected void AddDomainEvents(params DomainEvent[] domainEvents) => _domainEvents.AddRange(domainEvents);
}
