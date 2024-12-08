using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class LongCastleMoveStrategy : IMoveStrategy
{
    private readonly Chessboard _chessboard;

    public LongCastleMoveStrategy(Chessboard chessboard)
    {
        _chessboard = chessboard;
    }
    
    public bool IsValidMove()
    {
        return !KingHasMovedAlready() && !RookHasMovedAlready();
    }

    public void Execute()
    {
        var initialKingRow = _chessboard.CurrentTurnColour == Colour.White ? Row.One : Row.Eight;
        
        var kingInitialPosition = Square.At(Column.E, initialKingRow);
        var rookInitialPosition = Square.At(Column.A, initialKingRow);
        var kingDestination = Square.At(Column.C, initialKingRow);
        var rookDestination = Square.At(Column.D, initialKingRow);

        var king = _chessboard.GetPieceAt(kingInitialPosition);
        var rook = _chessboard.GetPieceAt(rookInitialPosition);
        
        _chessboard.Pieces.Remove(kingInitialPosition);
        _chessboard.Pieces.Remove(rookInitialPosition);
        
        king.MoveTo(kingDestination);
        rook.MoveTo(rookDestination);

        _chessboard.Pieces[king.Square] = king;
        _chessboard.Pieces[rook.Square] = rook;
    }
    
    private bool KingHasMovedAlready()
    {
        return _chessboard.Moves.Any(x => x.Colour == _chessboard.CurrentTurnColour && x.PieceType is PieceType.King);
    }

    private bool RookHasMovedAlready()
    {
        return _chessboard.Moves.Any(x => x.Colour == _chessboard.CurrentTurnColour && x.PieceType is PieceType.Rook);
    }
}