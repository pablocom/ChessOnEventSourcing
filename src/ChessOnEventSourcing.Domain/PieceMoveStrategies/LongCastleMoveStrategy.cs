namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class LongCastleMoveStrategy : IMoveStrategy
{
    private readonly Chessboard _chessboard;

    public LongCastleMoveStrategy(Chessboard chessboard)
    {
        _chessboard = chessboard;
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