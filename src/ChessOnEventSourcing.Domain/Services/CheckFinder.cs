using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Services;

internal static class CheckFinder
{
    public static bool IsCheckMateFrom(Colour currentTurnColour, IReadOnlyDictionary<Square, Piece> pieces)
    {
        if (!IsCheckFrom(currentTurnColour, pieces))
            return false;

        var opponentPieces = pieces.Values.Where(p => p.Colour == currentTurnColour.Opposite());
        foreach (var piece in opponentPieces)
        {
            var possibleMoves = piece.GetAvailableMoves(pieces);

            if (possibleMoves.Any(destination => MoveAvoidsCheck(piece, destination, currentTurnColour, pieces)))
                return false;
        }

        return true;
    }

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

    private static bool MoveAvoidsCheck(Piece piece, Square destination, Colour currentTurnColour, IReadOnlyDictionary<Square, Piece> pieces)
    {
        var boardCopy = new Dictionary<Square, Piece>(pieces);
        boardCopy.Remove(piece.Square);

        if (boardCopy.TryGetValue(destination, out var targetPiece))
        {
            if (targetPiece.Colour == piece.Colour)
                return false;

            boardCopy.Remove(destination);
        }

        var movedPiece = piece.CloneWithSquare(destination);
        boardCopy.Add(movedPiece.Square, movedPiece);

        return !IsCheckFrom(currentTurnColour, boardCopy);
    }
}