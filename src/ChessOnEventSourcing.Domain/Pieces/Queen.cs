using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Pieces;

public sealed class Queen : Piece
{
    private static readonly (int, int)[] AllDirections = [(1, 0), (-1, 0), (0, 1), (0, -1), (-1, -1), (1, 1), (1, -1), (-1, 1)];

    public Queen(Square initialSquare, Colour colour) : base(PieceType.Queen, initialSquare, colour)
    {
    }

    public override IReadOnlySet<Square> GetAvailableMoves(IReadOnlyDictionary<Square, Piece> board)
    {
        var availableMoves = new HashSet<Square>();
        
        foreach (var (columnDirection, rowDirection) in AllDirections)
        {
            if (Square.WouldOverflowAdding(columnDirection, rowDirection))
                continue;
            
            AddAvailableSquaresForDirection(columnDirection, rowDirection, board, availableMoves);
        }
        
        return availableMoves;
    }

    private void AddAvailableSquaresForDirection(int columnDirection, int rowDirection, IReadOnlyDictionary<Square, Piece> currentPieces, HashSet<Square> moves)
    {
        var destinationSquare = Square;
        while (!destinationSquare.WouldOverflowAdding(columnDirection, rowDirection))
        {
            destinationSquare = destinationSquare.Add(columnDirection, rowDirection);

            if (currentPieces.TryGetValue(destinationSquare, out var piece))
            {
                if (Colour != piece.Colour)
                    moves.Add(destinationSquare);
                
                break;
            }

            moves.Add(destinationSquare);
        }
    }

    public override Piece CloneWithSquare(Square destination) => new Queen(destination, Colour);
}