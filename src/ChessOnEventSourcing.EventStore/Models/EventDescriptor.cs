namespace ChessOnEventSourcing.EventStore.Models;

public sealed class EventDescriptor
{
    public Guid EventId { get; init; }
    public Guid AggregateId { get; init; }
    public string AggregateType { get; init; } = default!;
    public string EventType { get; init; } = default!;
    public string EventData { get; init; } = default!;
    public int Version { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}
