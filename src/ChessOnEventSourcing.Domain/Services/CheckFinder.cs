using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Services;

public static class CheckFinder
{
    public static bool IsCheckFrom(Colour currentTurnColour, IReadOnlyDictionary<Square, Piece> pieces)
    {
        var oppositeKing = pieces.Values.First(x => x.Colour == currentTurnColour.Opposite() && x.Type is PieceType.King);
        
        var ownColourPieces = pieces.Values.Where(x => x.Colour == currentTurnColour);
        foreach (var piece in ownColourPieces)
        {
            var availableMoves = piece.GetAvailableMoves(pieces);

            if (availableMoves.Contains(oppositeKing.Square)) 
                return true;
        }

        return false;
    }
}