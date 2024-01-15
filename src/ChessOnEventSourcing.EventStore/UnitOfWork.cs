using ChessOnEventSourcing.Application;
using System.Data.Common;

namespace ChessOnEventSourcing.EventStore;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private DbTransaction? _dbTransaction;

    public UnitOfWork(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task BeginTransaction(CancellationToken ct = default)
    {
        if (_dbTransaction is not null)
            throw new InvalidOperationException("Another transaction already started. Multiple transactions are not supported.");

        var connection = await _dbConnectionFactory.CreateConnectionAsync(ct);
        _dbTransaction = await connection.BeginTransactionAsync(ct);
    }

    public async Task Commit(CancellationToken ct = default)
    {
        if (_dbTransaction is null)
            throw new InvalidOperationException("Cannot commit because no transaction was opened. A transaction must be open before committing");

        await _dbTransaction.CommitAsync(ct);
    }

    public async Task Rollback(CancellationToken ct = default)
    {
        if (_dbTransaction is null)
            throw new InvalidOperationException("Cannot rollback because no transaction was opened. A transaction must be open before rollback");

        await _dbTransaction.RollbackAsync(ct);
    }

    public DbConnection GetDbConnection()
    {
        if (_dbTransaction is null)
            throw new InvalidOperationException("Cannot retrieve database connection because no transaction was opened");

        return _dbTransaction.Connection!;
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbTransaction is null)
            return;

        await _dbTransaction.DisposeAsync();
    }
}