using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public interface IReadOnlyPiece
{
    PieceType Type { get; }
    Square Square { get; }
    Colour Colour { get; }
}