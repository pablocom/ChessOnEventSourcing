using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class ShortCastleMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsApplicableTo(Chessboard chessboard, Square origin, Square destination, PieceType? _ = null)
    {
        var kingsInitialRow = chessboard.CurrentTurnColour == Colour.White ? Row.One : Row.Eight;

        if (!IsShortCastleMove(origin, destination, kingsInitialRow)) 
            return false;

        if (!chessboard.TryGetPieceAt(Square.At(Column.E, kingsInitialRow), out var piece))
            return false;
        
        if (piece.Type != PieceType.King || piece.Colour != chessboard.CurrentTurnColour)
            return false;

        var kingMovedPreviously = chessboard.Moves.Any(m => m.Colour == chessboard.CurrentTurnColour && m.PieceType == PieceType.King);
        if (kingMovedPreviously)
            return false;

        var rookPosition = Square.At(Column.H, kingsInitialRow);
        if (!chessboard.TryGetPieceAt(rookPosition, out var rook) || rook.Type != PieceType.Rook || rook.Colour != chessboard.CurrentTurnColour)
            return false;

        var rookMovedPreviously = chessboard.Moves.Any(m => m.Colour == chessboard.CurrentTurnColour && m.PieceType == PieceType.Rook && m.Origin == rookPosition);
        return !rookMovedPreviously;
    }

    private static bool IsShortCastleMove(Square origin, Square destination, Row kingsInitialRow)
    {
        var shortCastleOrigin = Square.At(Column.E, kingsInitialRow);
        var shortCastleDestination = Square.At(Column.G, kingsInitialRow);

        return origin == shortCastleOrigin && destination == shortCastleDestination;
    }
}