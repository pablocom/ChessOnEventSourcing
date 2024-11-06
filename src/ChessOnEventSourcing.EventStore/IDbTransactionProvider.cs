using System.Data.Common;

namespace ChessOnEventSourcing.EventStore;

public interface IDbTransactionProvider
{
    DbTransaction? GetCurrentTransaction();
}
