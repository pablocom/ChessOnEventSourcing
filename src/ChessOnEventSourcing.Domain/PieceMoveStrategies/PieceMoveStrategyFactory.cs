using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public static class PieceMoveStrategyFactory
{
    public static IPieceMoveStrategy Create(Chessboard chessboard, Square origin, Square destination)
    {
        return new NormalPieceMoveStrategy();
    }
}