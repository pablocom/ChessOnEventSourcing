
namespace ChessOnEventSourcing.Domain;

public enum PieceType
{
    Pawn, Rook, Knight, Bishop, Queen, King
}

public abstract class Piece : ValueObject
{
    public PieceType Type { get; protected init; }
    public Position Position { get; protected set; }
    public Colour Colour { get; protected init; }

    protected Piece(PieceType type, Position initialPosition, Colour colour)
    {
        Type = type;
        Position = initialPosition;
        Colour = colour;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Position;
        yield return Colour;
        yield return Type;
    }

    public void MoveTo(Position destination)
    {
        Position = destination;
    }
}

public class Pawn : Piece
{
    public Pawn(Position initialPosition, Colour colour) : base(PieceType.Pawn, initialPosition, colour) 
    { 
    }
}

public class Rook : Piece
{
    public Rook(Position initialPosition, Colour colour)
        : base(PieceType.Rook, initialPosition, colour)
    {
    }
}

public class Knight : Piece
{
    public Knight(Position initialPosition, Colour colour)
        : base(PieceType.Knight, initialPosition, colour)
    {
    }
}

public class Bishop : Piece
{
    public Bishop(Position initialPosition, Colour colour)
        : base(PieceType.Bishop, initialPosition, colour)
    {
    }

}

public class Queen : Piece
{
    public Queen(Position initialPosition, Colour colour)
        : base(PieceType.Queen, initialPosition, colour)
    {
    }

}

public class King : Piece
{
    public King(Position initialPosition, Colour colour)
        : base(PieceType.King, initialPosition, colour)
    {
    }
}
