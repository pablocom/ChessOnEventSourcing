using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public sealed class Rook : Piece
{
    private static readonly int[] Directions = [1, -1];
    
    public Rook(Square initialSquare, Colour colour) : base(PieceType.Rook, initialSquare, colour)
    {
    }

    public override IReadOnlySet<Square> GetAvailableMoves(IReadOnlyDictionary<Square, Piece> board)
    {
        var moves = new HashSet<Square>();

        foreach (var direction in Directions)
        {
            AddVerticalMoves(board, direction, moves);
            AddHorizontalMoves(board, direction, moves);
        }
        
        return moves;
    }

    private void AddVerticalMoves(IReadOnlyDictionary<Square, Piece> board, int direction, HashSet<Square> moves)
    {
        var nextSquare = Square;

        while (!nextSquare.WouldOverflowAddingRow(direction))
        {
            nextSquare = nextSquare.AddRows(direction);
                    
            if (board.TryGetValue(nextSquare, out var piece))
            {
                if (Colour != piece.Colour)
                    moves.Add(nextSquare);
                
                break;
            }

            moves.Add(nextSquare);
        }
    }
    
    private void AddHorizontalMoves(IReadOnlyDictionary<Square, Piece> board, int direction, HashSet<Square> moves)
    {
        var nextSquare = Square;

        while (!nextSquare.WouldOverflowAddingColumn(direction))
        {
            nextSquare = nextSquare.AddColumns(direction);
                    
            if (board.TryGetValue(nextSquare, out var piece))
            {
                if (Colour != piece.Colour)
                    moves.Add(nextSquare);
                
                break;
            }

            moves.Add(nextSquare);
        }
    }
    
    public override Piece CloneWithSquare(Square destination) => new Rook(destination, Colour);
}