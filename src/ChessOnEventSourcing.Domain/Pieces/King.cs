using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public sealed class King : Piece
{
    private static readonly (int, int)[] PotentialMoves =
    [
        (1, 0), (1, 1), (0, 1), (-1, 1), 
        (-1, 0), (-1, -1), (0, -1), (1, -1)
    ];

    public King(Square initialSquare, Colour colour) : base(PieceType.King, initialSquare, colour)
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

            if (board.TryGetValue(nextSquare, out var piece) && piece.Colour == this.Colour)
                continue;

            moves.Add(nextSquare);
        }

        return moves;
    }

    public bool IsBeingChecked(IReadOnlyDictionary<Square, Piece> board)
    {
        var pieces = board.Values.Where(x => x.Colour == Colour.Opposite()).ToArray();
        
        foreach (var piece in pieces)
        {
            var availableMoves = piece.GetAvailableMoves(board);

            if (!availableMoves.Contains(Square))
                continue;

            var kingMoves = GetAvailableMoves(board);
            
            if (!AnyLegalMove(board, kingMoves, pieces)) 
                return false;
        }

        return true;
    }

    private bool AnyLegalMove(IReadOnlyDictionary<Square, Piece> board, IEnumerable<Square> kingMoves, Piece[] pieces)
    {
        foreach (var kingMove in kingMoves)
        {
            var piecesWithMovedKing = new Dictionary<Square, Piece>(board);
            piecesWithMovedKing.Remove(Square);
            piecesWithMovedKing[kingMove] = this;
                
            var canEscape = true;
            foreach (var piece in pieces)
            {
                if (!piece.GetAvailableMoves(piecesWithMovedKing).Contains(kingMove)) 
                    continue;
                    
                canEscape = false;
                break;
            }
                
            if (canEscape) 
                return false;
        }

        return true;
    }

    public override Piece CloneWithSquare(Square destination) => new King(destination, Colour);
}