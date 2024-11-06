using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.Services;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class EnPassantMoveStrategy : IPieceMoveStrategy
{
    private readonly Chessboard _chessboard;
    private readonly Square _origin;
    private readonly Square _destination;

    public EnPassantMoveStrategy(Chessboard chessboard, Square origin, Square destination)
    {
        _chessboard = chessboard;
        _origin = origin;
        _destination = destination;
    }
    
    public bool IsValidMove()
    {
        var piece = _chessboard.Pieces[_origin];
        if (!piece.GetAvailableMoves(_chessboard.Pieces).Contains(_destination))
            return false;
        
        var boardWithPieceMoved = new Dictionary<Square, Piece>(_chessboard.Pieces);
        
        var movedPiece = boardWithPieceMoved[_origin].CloneWithSquare(_destination);
        boardWithPieceMoved.Remove(_origin);
        movedPiece.MoveTo(_destination);
        boardWithPieceMoved[movedPiece.Square] = movedPiece;
        
        var capturedPawnSquare = Square.At(_destination.Column, _origin.Row);
        boardWithPieceMoved.Remove(capturedPawnSquare);
        
        return CheckFinder.IsCheckFrom(_chessboard.CurrentTurnColour.Opposite(), boardWithPieceMoved);
    }

    public void Execute()
    {
        var capturedPawnSquare = Square.At(_destination.Column, _origin.Row); 
        _chessboard.KilledPieces.Add(_chessboard.Pieces[capturedPawnSquare]);
        _chessboard.Pieces.Remove(capturedPawnSquare);
        
        var pawn = _chessboard.Pieces[_origin];
        pawn.MoveTo(_destination);
        _chessboard.Pieces.Remove(_origin);
        _chessboard.Pieces[pawn.Square] = pawn;
    }
}