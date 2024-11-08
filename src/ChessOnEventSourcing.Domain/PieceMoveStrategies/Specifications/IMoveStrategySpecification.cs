using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public interface IMoveStrategySpecification
{
    bool IsSatisfiedBy(Chessboard chessboard, Square origin, Square destination);
}