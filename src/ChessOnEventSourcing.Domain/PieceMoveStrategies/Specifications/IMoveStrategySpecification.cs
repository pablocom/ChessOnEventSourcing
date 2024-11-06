using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public interface IMoveStrategySpecification
{
    bool IsApplicableTo(Chessboard chessboard, Square origin, Square destination, PieceType? pieceType = null);
}