namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public interface IMoveStrategy
{
    bool IsValidMove();
    void Execute();
}