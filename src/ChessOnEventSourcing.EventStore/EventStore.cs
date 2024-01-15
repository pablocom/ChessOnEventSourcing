using ChessOnEventSourcing.Domain;

namespace ChessOnEventSourcing.EventStore;

public interface IEventStore
{
    Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId);
    Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId, DateTimeOffset date);
    Task Save<TAggregateRoot, TId>(TAggregateRoot aggregate) 
        where TAggregateRoot : AggregateRoot<TId> 
        where TId : notnull, IFormattable;
}

public sealed class EventStore : IEventStore
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public EventStore(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId) => GetEvents(aggregateId, DateTimeOffset.MaxValue);

    public Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId, DateTimeOffset date)
    {
        throw new NotImplementedException();
    }

    public Task Save<TAggregateRoot, TId>(TAggregateRoot aggregate)
        where TAggregateRoot : AggregateRoot<TId>
        where TId : notnull, IFormattable
    {
        throw new NotImplementedException();
    }
}
