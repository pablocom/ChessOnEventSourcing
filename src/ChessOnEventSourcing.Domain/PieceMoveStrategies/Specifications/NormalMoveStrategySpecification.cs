using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class NormalMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsApplicableTo(Chessboard chessboard, Square origin, Square destination, PieceType? _ = null)
    {
        return true;
    }
}