using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class NormalMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsSatisfiedBy(Chessboard chessboard, Square origin, Square destination)
    {
        return true;
    }
}