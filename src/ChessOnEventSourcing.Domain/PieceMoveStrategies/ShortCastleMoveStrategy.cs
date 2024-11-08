using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class ShortCastleMoveStrategy : IMoveStrategy
{
    private readonly Chessboard _chessboard;

    public ShortCastleMoveStrategy(Chessboard chessboard)
    {
        _chessboard = chessboard;
    }
    
    public bool IsValidMove()
    {
        if (KingHasMovedAlready(_chessboard))
            return false;
        
        if (RookHasMovedAlready(_chessboard))
            return false;

        return true;
    }

    public void Execute()
    {
        var initialRow = _chessboard.CurrentTurnColour == Colour.White ? Row.One : Row.Eight;
        var initialKingSquare = Square.At(Column.E, initialRow);
        var initialRookSquare = Square.At(Column.H, initialRow);

        var kingDestination = Square.At(Column.G, initialRow);
        var rookDestination = Square.At(Column.F, initialRow);

        var king = _chessboard.Pieces[initialKingSquare];
        var rook = _chessboard.Pieces[initialRookSquare];

        _chessboard.Pieces.Remove(initialKingSquare);
        _chessboard.Pieces.Remove(initialRookSquare);
        king.MoveTo(kingDestination);
        rook.MoveTo(rookDestination);
        _chessboard.Pieces.Add(king.Square, king);
        _chessboard.Pieces.Add(rook.Square, rook);
    }

    private static bool KingHasMovedAlready(Chessboard chessboard)
    {
        return chessboard.Moves.Any(x => x.Colour == chessboard.CurrentTurnColour && x.PieceType is PieceType.King);
    }

    private static bool RookHasMovedAlready(Chessboard chessboard)
    {
        return chessboard.Moves.Any(x => x.Colour == chessboard.CurrentTurnColour && x.PieceType is PieceType.Rook);
    }
}