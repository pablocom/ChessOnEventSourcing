using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Models;

namespace ChessOnEventSourcing.EventStore;

public interface IEventStore
{
    Task Save(AggregateRoot aggregate, CancellationToken ct = default);
    Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId, CancellationToken ct = default);
    Task<IEnumerable<EventDescriptor>> GetEventsUntilDate(Guid aggregateId, DateTimeOffset date, CancellationToken ct = default);
}
