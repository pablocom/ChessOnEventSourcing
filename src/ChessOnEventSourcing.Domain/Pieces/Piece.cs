
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public abstract class Piece : Entity, IReadOnlyPiece
{
    public PieceType Type { get; }
    public Square Square { get; private set; }
    public Colour Colour { get; }

    protected Piece(PieceType type, Square initialSquare, Colour colour)
    {
        Id = Guid.NewGuid();
        Type = type;
        Square = initialSquare;
        Colour = colour;
    }

    public void MoveTo(Square destination)
    {
        Square = destination;
    }

    public abstract IReadOnlySet<Square> GetAvailableMoves(IReadOnlyDictionary<Square, Piece> board);

    public abstract Piece CloneWithSquare(Square destination);
}