using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class ShortCastleMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsSatisfiedBy(Chessboard chessboard, Square origin, Square destination)
    {
        var kingsInitialRow = chessboard.CurrentTurnColour == Colour.White ? Row.One : Row.Eight;

        if (!IsShortCastleMove(origin, destination, kingsInitialRow)) 
            return false;

        if (!chessboard.TryGetPieceAt(Square.At(Column.E, kingsInitialRow), out var piece))
            return false;
        
        if (piece.Type != PieceType.King || piece.Colour != chessboard.CurrentTurnColour)
            return false;

        var rookPosition = Square.At(Column.H, kingsInitialRow);
        if (!chessboard.TryGetPieceAt(rookPosition, out var rook) || rook.Type != PieceType.Rook || rook.Colour != chessboard.CurrentTurnColour)
            return false;

        return true;
    }

    private static bool IsShortCastleMove(Square origin, Square destination, Row kingsInitialRow)
    {
        var shortCastleOrigin = Square.At(Column.E, kingsInitialRow);
        var shortCastleDestination = Square.At(Column.G, kingsInitialRow);

        return origin == shortCastleOrigin && destination == shortCastleDestination;
    }
}