using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.Services;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class NormalPieceMoveStrategy : IPieceMoveStrategy
{
    public bool IsApplicable(Chessboard chessboard, Square origin, Square destination)
    {
        return true;
    }

    public bool IsValidMove(Chessboard chessboard, Square origin, Square destination)
    {
        var piece = chessboard.Pieces[origin];
        if (!piece.GetAvailableMoves(chessboard.Pieces).Contains(destination))
            return false;
        
        var boardWithPieceMoved = new Dictionary<Square, Piece>(chessboard.Pieces);
        
        var movedPiece = boardWithPieceMoved[origin].CloneWithSquare(destination);
        boardWithPieceMoved.Remove(origin);
        movedPiece.MoveTo(destination);
        boardWithPieceMoved[movedPiece.Square] = movedPiece;

        return CheckFinder.IsCheckFrom(chessboard.CurrentTurnColour.Opposite(), boardWithPieceMoved);
    }

    public void Execute(Chessboard chessboard, Square origin, Square destination)
    {
        if (chessboard.Pieces.TryGetValue(destination, out var killedPiece))
        {
            chessboard.Pieces.Remove(killedPiece.Square);
            chessboard.KilledPieces.Add(killedPiece);
        }
        
        var piece = chessboard.Pieces[origin];
        piece.MoveTo(destination);

        chessboard.Pieces.Remove(origin);
        chessboard.Pieces[piece.Square] = piece;
    }
}