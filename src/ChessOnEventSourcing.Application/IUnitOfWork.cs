namespace ChessOnEventSourcing.Application;

public interface IUnitOfWork
{
    Task BeginTransaction(CancellationToken ct = default);
    Task Commit(CancellationToken ct = default);
    Task Rollback(CancellationToken ct = default);
}