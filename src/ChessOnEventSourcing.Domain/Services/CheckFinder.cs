using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Services;

public static class CheckFinder
{
    public static bool IsCheckFrom(Colour colour, IReadOnlyDictionary<Square, Piece> pieces)
    {
        var oppositeKing = pieces.Values.First(x => x.Colour == colour.Opposite() && x.Type is PieceType.King);
        var ownColourPieces = pieces.Values.Where(x => x.Colour == colour);
        
        foreach (var piece in ownColourPieces)
        {
            var availableMoves = piece.GetAvailableMoves(pieces);

            if (availableMoves.Contains(oppositeKing.Square)) 
                return true;
        }

        return false;
    }
}