using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public class Pawn : Piece
{
    private readonly Row _initialRow;
    private readonly int _direction;

    public Pawn(Square initialSquare, Colour colour) : base(PieceType.Pawn, initialSquare, colour)
    {
        _direction = Colour == Colour.White ? 1 : -1;
        _initialRow = Colour == Colour.White ? Row.Two : Row.Seven;
    }

    public override IReadOnlySet<Square> GetAvailableMoves(IReadOnlyDictionary<Square, Piece> board)
    {
        var availableDestinations = new HashSet<Square>();

        var oneStepForward = Square.AddRows(_direction);
        if (!board.ContainsKey(oneStepForward))
        {
            availableDestinations.Add(oneStepForward);

            if (Square.Row == _initialRow)
            {
                var twoStepsForward = Square.AddRows(_direction * 2);
                if (!board.ContainsKey(twoStepsForward))
                {
                    availableDestinations.Add(twoStepsForward);
                }
            }
        }

        if (!Square.WouldOverflowAdding(-1, _direction))
        {
            var captureLeft = Square.Add(-1, _direction);
            if (board.TryGetValue(captureLeft, out var leftPiece) && leftPiece.Colour != this.Colour)
                availableDestinations.Add(captureLeft);
        }

        if (!Square.WouldOverflowAdding(1, _direction))
        {
            var captureRight = Square.Add(1, _direction);
            if (board.TryGetValue(captureRight, out var rightPiece) && rightPiece.Colour != this.Colour)
                availableDestinations.Add(captureRight);
        }

        return availableDestinations;
    }
    
    public override Piece CloneWithSquare(Square destination) => new Pawn(destination, Colour);
}
