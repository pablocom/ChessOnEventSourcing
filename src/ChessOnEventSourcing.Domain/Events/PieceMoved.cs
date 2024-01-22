namespace ChessOnEventSourcing.Domain.Events;

public sealed record PieceMoved(
    Guid AggregateId,
    PieceType PieceType,
    char OriginColumn,
    int OriginRow,
    char DestinationColumn,
    int DestinationRow) : Event(AggregateId);
