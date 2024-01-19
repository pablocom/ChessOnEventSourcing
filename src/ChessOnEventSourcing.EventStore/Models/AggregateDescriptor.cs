namespace ChessOnEventSourcing.EventStore.Models;

public sealed record AggregateDescriptor(
    Guid AggregateId,
    string AggregateType,
    int Version);
