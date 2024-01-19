using ChessOnEventSourcing.Application;
using ChessOnEventSourcing.Domain;
using Dapper;
using NpgsqlTypes;
using System.Text.Json;

namespace ChessOnEventSourcing.EventStore;

public sealed class NpgsqlEventStore : IEventStore
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public NpgsqlEventStore(IUnitOfWork unitOfWork, IDbConnectionFactory dbConnectionFactory)
    {
        _unitOfWork = unitOfWork;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Save(AggregateRoot aggregate, CancellationToken ct = default)
    {
        var aggregateType = aggregate.GetType().FullName!;
        var aggregateEvents = aggregate.DomainEvents;

        var eventDescriptors = new List<EventDescriptor>(aggregateEvents.Count);
        foreach (var @event in aggregateEvents)
        {
            var eventType = @event.GetType();

            eventDescriptors.Add(new EventDescriptor
            {
                EventId = Guid.NewGuid(),
                AggregateId = @event.AggregateId,
                AggregateType = aggregateType,
                EventType = eventType.FullName!,
                EventData = JsonSerializer.Serialize(@event, eventType),
                OccurredOn = DateTimeOffset.UtcNow
            });
        }

        await using var command = _unitOfWork.CreateCommand(ct);

        command.CommandText = "CALL save_aggregate(@AggregateId, @AggregateType, @ExpectedVersion, @Events)";

        command.Parameters.AddWithValue("AggregateId", aggregate.Id);
        command.Parameters.AddWithValue("AggregateType", aggregateType);
        command.Parameters.AddWithValue("ExpectedVersion", aggregate.Version);
        command.Parameters.AddWithValue("Events", NpgsqlDbType.Json, JsonSerializer.Serialize(eventDescriptors));

        await using var resultReader = await command.ExecuteReaderAsync(ct);
    }

    public Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId, CancellationToken ct = default) 
        => GetEventsUntilDate(aggregateId, DateTimeOffset.MaxValue, ct);

    public async Task<IEnumerable<EventDescriptor>> GetEventsUntilDate(Guid aggregateId, DateTimeOffset date, CancellationToken ct = default)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var result = await connection.QueryAsync<EventDescriptor>("""
            SELECT "EventId", "AggregateId", "AggregateType", "EventType", "EventData", "Version", "OccurredOn" FROM "Events"
            WHERE "AggregateId" = @AggregateId AND "OccurredOn" <= @Date ORDER BY "Version" ASC;
            """,
            new { AggregateId = aggregateId, Date = date }
        );

        return result;
    }
}
