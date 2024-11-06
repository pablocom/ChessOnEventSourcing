using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.Services;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class NormalPieceMoveStrategy : IPieceMoveStrategy
{
    private readonly Chessboard _chessboard;
    private readonly Square _origin;
    private readonly Square _destination;

    public NormalPieceMoveStrategy(Chessboard chessboard, Square origin, Square destination)
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

        return CheckFinder.IsCheckFrom(_chessboard.CurrentTurnColour.Opposite(), boardWithPieceMoved);
    }

    public void Execute()
    {
        if (_chessboard.Pieces.TryGetValue(_destination, out var killedPiece))
        {
            _chessboard.Pieces.Remove(killedPiece.Square);
            _chessboard.KilledPieces.Add(killedPiece);
        }
        
        var piece = _chessboard.Pieces[_origin];
        piece.MoveTo(_destination);

        _chessboard.Pieces.Remove(_origin);
        _chessboard.Pieces[piece.Square] = piece;
    }
}