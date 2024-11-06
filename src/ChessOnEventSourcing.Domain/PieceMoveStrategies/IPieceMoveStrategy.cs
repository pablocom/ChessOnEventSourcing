using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public interface IPieceMoveStrategy
{
    bool IsApplicable(Chessboard chessboard, Square origin, Square destination);
    bool IsValidMove(Chessboard chessboard, Square origin, Square destination);
    void Execute(Chessboard chessboard, Square origin, Square destination);
}