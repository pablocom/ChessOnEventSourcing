using System.Data;
using System.Data.Common;

namespace ChessOnEventSourcing.EventStore;

public interface IDbConnectionFactory
{
    public Task<DbConnection> CreateConnectionAsync(CancellationToken ct = default);
}
