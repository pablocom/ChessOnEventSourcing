namespace ChessOnEventSourcing.Domain;

public sealed record ChessboardCreated(
    Guid AggregateId, 
    Guid CreatedBy,
    DateTimeOffset CreatedAt) : Event(AggregateId);

public sealed record ChessboardFinished(
    Guid AggregateId,
    DateTimeOffset FinishedAt) : Event(AggregateId);
