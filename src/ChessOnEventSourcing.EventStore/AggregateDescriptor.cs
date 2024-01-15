namespace ChessOnEventSourcing.EventStore;

public sealed record AggregateDescriptor(
    Guid AggregateId,
    string AggregateType,
    int Version);
