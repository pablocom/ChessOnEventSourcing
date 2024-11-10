using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.Services;
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
        if (KingHasMovedAlready())
            return false;
        
        if (RookHasMovedAlready())
            return false;

        if (AnyPieceTargetingSquaresBetweenKingAndRook())
            return false;

        if (CheckFinder.IsCheckFrom(_chessboard.CurrentTurnColour.Opposite(), SimulateBoardAfterShortCastle()))
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

    private bool KingHasMovedAlready()
    {
        return _chessboard.Moves.Any(x => x.Colour == _chessboard.CurrentTurnColour && x.PieceType is PieceType.King);
    }

    private bool RookHasMovedAlready()
    {
        return _chessboard.Moves.Any(x => x.Colour == _chessboard.CurrentTurnColour && x.PieceType is PieceType.Rook);
    }

    private bool AnyPieceTargetingSquaresBetweenKingAndRook()
    {
        var opponentPieces = _chessboard.Pieces.Values.Where(x => x.Colour == _chessboard.CurrentTurnColour.Opposite());

        foreach (var piece in opponentPieces)
        {
            var availableMoves = piece.GetAvailableMoves(_chessboard.Pieces);

            if (availableMoves.Contains(Square.Parse("F1")) || availableMoves.Contains(Square.Parse("G1")))
                return true;
        }

        return false;
    }


    private Dictionary<Square, Piece> SimulateBoardAfterShortCastle()
    {
        var boardCopy = new Dictionary<Square, Piece>(_chessboard.Pieces);

        var initialRow = _chessboard.CurrentTurnColour == Colour.White ? Row.One : Row.Eight;
        var initialKingSquare = Square.At(Column.E, initialRow);
        var initialRookSquare = Square.At(Column.H, initialRow);

        var kingDestination = Square.At(Column.G, initialRow);
        var rookDestination = Square.At(Column.F, initialRow);

        var king = boardCopy[initialKingSquare];
        var rook = boardCopy[initialRookSquare];

        boardCopy.Remove(initialKingSquare);
        boardCopy.Remove(initialRookSquare);

        king.CloneWithSquare(kingDestination);
        rook.CloneWithSquare(rookDestination);
        boardCopy.Add(king.Square, king);
        boardCopy.Add(rook.Square, rook);

        return boardCopy;
    }
}