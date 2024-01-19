using Npgsql;

namespace ChessOnEventSourcing.EventStore;

public interface IGetCurrentTransaction
{
    NpgsqlTransaction GetCurrentTransaction();
}
