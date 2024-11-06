using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class LongCastleMoveStrategy : IPieceMoveStrategy
{
    public bool IsApplicable(Chessboard chessboard, Square origin, Square destination)
    {
        throw new NotImplementedException();
    }

    public bool IsValidMove(Chessboard chessboard, Square origin, Square destination)
    {
        throw new NotImplementedException();
    }

    public void Execute(Chessboard chessboard, Square origin, Square destination)
    {
        throw new NotImplementedException();
    }
}