using System.Data;

namespace ChessOnEventSourcing.EventStore;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync();
}
