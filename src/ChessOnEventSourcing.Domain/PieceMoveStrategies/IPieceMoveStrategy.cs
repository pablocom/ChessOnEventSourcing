namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public interface IPieceMoveStrategy
{
    bool IsValidMove();
    void Execute();
}