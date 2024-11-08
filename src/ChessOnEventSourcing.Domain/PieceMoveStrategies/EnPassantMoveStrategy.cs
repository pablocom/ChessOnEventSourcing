using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.Services;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class EnPassantMoveStrategy : IMoveStrategy
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
        var simulatedBoardAfterMove = new Dictionary<Square, Piece>(_chessboard.Pieces);
        
        MovePiece(simulatedBoardAfterMove);
        RemoveCapturedPawn(simulatedBoardAfterMove);

        return !CheckFinder.IsCheckFrom(_chessboard.CurrentTurnColour.Opposite(), simulatedBoardAfterMove);
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
    
    private void MovePiece(Dictionary<Square, Piece> simulatedBoardBeforeMove)
    {
        var movedPiece = simulatedBoardBeforeMove[_origin].CloneWithSquare(_destination);
        simulatedBoardBeforeMove.Remove(_origin);
        movedPiece.MoveTo(_destination);
        simulatedBoardBeforeMove[movedPiece.Square] = movedPiece;
    }
    
    private void RemoveCapturedPawn(Dictionary<Square, Piece> simulatedBoardBeforeMove)
    {
        var capturedPawnSquare = Square.At(_destination.Column, _origin.Row);
        simulatedBoardBeforeMove.Remove(capturedPawnSquare);
    }
}