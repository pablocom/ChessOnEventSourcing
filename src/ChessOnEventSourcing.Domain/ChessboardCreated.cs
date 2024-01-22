namespace ChessOnEventSourcing.Domain;

public sealed record ChessboardCreated(
    Guid AggregateId, 
    Guid CreatedBy,
    DateTimeOffset CreatedAt) : Event(AggregateId);

public sealed record ChessboardFinished(
    Guid AggregateId,
    DateTimeOffset FinishedAt) : Event(AggregateId);

public sealed record PieceMoved(
    Guid AggregateId,
    PieceType PieceType,
    char OriginColumn,
    int OriginRow,
    char DestinationColumn,
    int DestinationRow) : Event(AggregateId);
