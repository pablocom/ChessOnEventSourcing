namespace ChessOnEventSourcing.Domain;

public sealed record ChessboardFinished(
    Guid AggregateId,
    DateTimeOffset FinishedAt) : Event(AggregateId);
