using Npgsql;

namespace ChessOnEventSourcing.EventStore;

public interface INpgsqlTransactionProvider
{
    NpgsqlTransaction? GetCurrentTransaction();
}
