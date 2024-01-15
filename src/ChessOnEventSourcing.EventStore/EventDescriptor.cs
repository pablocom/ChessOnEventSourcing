namespace ChessOnEventSourcing.EventStore;

public sealed record EventDescriptor(
    Guid EventId,
    Guid AggregateId,
    string AggregateType,
    string EventType,
    string EventData,
    int Version, 
    DateTimeOffset OccurredOn);
