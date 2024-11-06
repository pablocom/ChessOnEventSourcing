using ChessOnEventSourcing.Domain.Pieces;

namespace ChessOnEventSourcing.Domain.ValueObjects;

public readonly struct Move : IEquatable<Move>
{
    public PieceType PieceType { get; }
    public Colour Colour { get; }
    public Square Origin { get; }
    public Square Destination { get; }

    public Move(PieceType pieceType, Colour colour, Square origin, Square destination)
    {
        PieceType = pieceType;
        Colour = colour;
        Origin = origin;
        Destination = destination;
    }

    public bool Equals(Move other)
    {
        return PieceType == other.PieceType && 
               Origin == other.Origin && 
               Destination == other.Destination;
    }

    public override bool Equals(object? obj) => obj is Move other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PieceType, Origin, Destination);
}