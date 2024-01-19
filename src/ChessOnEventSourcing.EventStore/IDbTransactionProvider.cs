using Npgsql;

namespace ChessOnEventSourcing.EventStore;

public interface IDbTransactionProvider
{
    NpgsqlTransaction? GetCurrentTransaction();
}
