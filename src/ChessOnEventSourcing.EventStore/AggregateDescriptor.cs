namespace ChessOnEventSourcing.EventStore;

public sealed record AggregateDescriptor(
    Guid AggregateId,
    string Type,
    int Version);
