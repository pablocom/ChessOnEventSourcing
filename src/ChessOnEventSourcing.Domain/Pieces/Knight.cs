using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public sealed class Knight : Piece
{
    private static readonly (int, int)[] PotentialMoves =
    [
        (2, 1), (2, -1), (-2, 1), (-2, -1),
        (1, 2), (1, -2), (-1, 2), (-1, -2)
    ];

    public Knight(Square initialSquare, Colour colour) : base(PieceType.Knight, initialSquare, colour)
    {
    }

    public override IReadOnlySet<Square> GetAvailableMoves(IReadOnlyDictionary<Square, Piece> board)
    {
        var moves = new HashSet<Square>();
        
        foreach (var (columnDirection, rowDirection) in PotentialMoves)
        {
            if (Square.WouldOverflowAdding(columnDirection, rowDirection))
                continue;
            
            var nextSquare = Square.Add(columnDirection, rowDirection);

            if (board.TryGetValue(nextSquare, out var piece) && piece.Colour == Colour)
                continue;
            
            moves.Add(nextSquare);
        }

        return moves;
    }

    public override Piece CloneWithSquare(Square destination) => new Knight(destination, Colour);
}