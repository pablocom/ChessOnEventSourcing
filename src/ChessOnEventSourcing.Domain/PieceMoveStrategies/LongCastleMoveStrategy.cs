using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class LongCastleMoveStrategy : IPieceMoveStrategy
{
    private readonly Chessboard _chessboard;
    private readonly Square _origin;
    private readonly Square _destination;

    public LongCastleMoveStrategy(Chessboard chessboard, Square origin, Square destination)
    {
        _chessboard = chessboard;
        _origin = origin;
        _destination = destination;
    }
    
    public bool IsValidMove()
    {
        throw new NotImplementedException();
    }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}