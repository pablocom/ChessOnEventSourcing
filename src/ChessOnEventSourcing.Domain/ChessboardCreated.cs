namespace ChessOnEventSourcing.Domain;

public sealed record ChessboardCreated(
    Guid AggregateId, 
    Guid CreatedBy,
    DateTimeOffset CreatedAt) : DomainEvent(AggregateId);

public sealed record ChessboardFinished(
    Guid AggregateId,
    DateTimeOffset FinishedAt) : DomainEvent(AggregateId);
