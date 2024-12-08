using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class LongCastleMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsSatisfiedBy(Chessboard chessboard, Square origin, Square destination)
    {
        var initialKingRow = chessboard.CurrentTurnColour == Colour.White ? Row.One : Row.Eight;

        if (!IsLongCastleMove(origin, destination, initialKingRow))
            return false;

        return true;
    }

    private static bool IsLongCastleMove(Square origin, Square destination, Row initialKingRow)
    {
        var longCastleOrigin = Square.At(Column.E, initialKingRow);
        var longCastleDestination = Square.At(Column.C, initialKingRow);

        return longCastleOrigin == origin && longCastleDestination == destination;
    }
}