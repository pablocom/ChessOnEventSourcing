using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public sealed class Bishop : Piece
{
    private static readonly (int, int)[] DiagonalDirections = [(1, 1), (1, -1), (-1, -1), (-1, 1)];
    
    public Bishop(Square initialSquare, Colour colour) : base(PieceType.Bishop, initialSquare, colour)
    {
    }

    public override IReadOnlySet<Square> GetAvailableMoves(IReadOnlyDictionary<Square, Piece> board)
    {
        var moves = new HashSet<Square>();
        
        foreach (var (rowDirection, columnDirection) in DiagonalDirections)
        {
            var nextSquare = Square;
            
            while (!nextSquare.WouldOverflowAdding(rowDirection, columnDirection))
            {
                nextSquare = nextSquare.Add(rowDirection, columnDirection);

                if (board.TryGetValue(nextSquare, out var piece))
                {
                    if (Colour != piece.Colour)
                        moves.Add(nextSquare);
                
                    break;
                }

                moves.Add(nextSquare);
            }            
        }

        return moves;
    }

    public override Piece CloneWithSquare(Square destination) => new Bishop(destination, Colour);
}