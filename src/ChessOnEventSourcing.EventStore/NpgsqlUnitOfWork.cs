using ChessOnEventSourcing.Application;
using Npgsql;
using System.Data.Common;

namespace ChessOnEventSourcing.EventStore;

public sealed class NpgsqlUnitOfWork : IUnitOfWork, IDbTransactionProvider, IAsyncDisposable
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private DbConnection? _dbConnection;
    private DbTransaction? _dbTransaction;

    public NpgsqlUnitOfWork(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task BeginTransaction(CancellationToken ct = default)
    {
        if (_dbTransaction is not null)
            throw new InvalidOperationException("Another transaction already started. Multiple transactions are not supported.");

        _dbConnection = await _dbConnectionFactory.CreateConnectionAsync(ct);
        _dbTransaction = await _dbConnection.BeginTransactionAsync(ct);
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


    public DbTransaction? GetCurrentTransaction() => _dbTransaction;

    public async ValueTask DisposeAsync()
    {
        if (_dbTransaction is not null)
            await _dbTransaction.DisposeAsync();

        if (_dbConnection is not null)
            await _dbConnection.DisposeAsync();
    }
}