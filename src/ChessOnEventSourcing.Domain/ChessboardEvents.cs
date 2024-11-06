using ChessOnEventSourcing.Domain.Pieces;

namespace ChessOnEventSourcing.Domain;

public abstract record Event(Guid AggregateId);

public sealed record ChessboardCreated(
    Guid ChessboardId, 
    DateTimeOffset CreatedAt) : Event(ChessboardId);

public sealed record ChessboardFinished(
    Guid ChessboardId,
    DateTimeOffset FinishedAt) : Event(ChessboardId);

public sealed record PieceMoved(
    Guid ChessboardId,
    PieceType PieceType,
    char OriginColumn,
    int OriginRow,
    char DestinationColumn,
    int DestinationRow) : Event(ChessboardId);
