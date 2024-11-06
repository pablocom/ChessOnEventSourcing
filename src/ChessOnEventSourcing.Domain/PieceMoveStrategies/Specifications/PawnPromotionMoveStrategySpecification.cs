using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public static class PawnPromotionMoveStrategySpecification
{
    public static bool IsApplicableTo(Chessboard chessboard, Square origin, Square destination, PieceType typeToBePromoted)
    {
        throw new NotImplementedException();
    }
}