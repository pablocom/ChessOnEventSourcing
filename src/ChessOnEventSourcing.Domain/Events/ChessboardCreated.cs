namespace ChessOnEventSourcing.Domain.Events;

public sealed record ChessboardCreated(
    Guid AggregateId,
    Guid CreatedBy,
    DateTimeOffset CreatedAt) : Event(AggregateId);
