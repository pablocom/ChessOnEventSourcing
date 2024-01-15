namespace ChessOnEventSourcing.Domain;

public abstract class AggregateRoot : Entity
{
    public int Version { get; protected set; }
    
    private readonly List<Event> _events = [];
    public IReadOnlyList<Event> Events => _events.ToArray().AsReadOnly();
    
    public void ClearEvents() => _events.Clear();
    public abstract void Apply(Event @event);

    protected void AddEvent(Event @event) => _events.Add(@event);
    protected void AddEvents(IEnumerable<Event> domainEvents) => _events.AddRange(domainEvents);
}
