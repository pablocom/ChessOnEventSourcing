using ChessOnEventSourcing.Domain;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ChessOnEventSourcing.EventStore;

public interface IEventStore
{
    Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId);
    Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId, DateTimeOffset date);
    Task Save(AggregateRoot aggregate);
}

public sealed class EventStore : IEventStore
{
    private readonly UnitOfWork _unitOfWork;

    public EventStore(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId) => GetEvents(aggregateId, DateTimeOffset.MaxValue);

    public async Task<IEnumerable<EventDescriptor>> GetEvents(Guid aggregateId, DateTimeOffset date)
    {
        var connection = _unitOfWork.GetDbConnection();

        var result = await connection.QueryAsync<EventDescriptor>("""
            SELECT "EventId", "AggregateId", "AggregateType", "EventType", "EventData", "Version", "OccurredOn" FROM "Events"
            WHERE "AggregateId" = @AggregateId "OccurredOn" <= @Date AND ORDER BY "Version" ASC;
            """,
            new { AggregateId = aggregateId, Date = date }
        );

        return result;
    }

    public async Task Save(AggregateRoot aggregate)
    {
        var connection = _unitOfWork.GetDbConnection();

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

        await using var command = new NpgsqlCommand("CALL save_aggregate(@AggregateId, @AggregateType, @ExpectedVersion, @Events)");
        command.Parameters.AddWithValue("AggregateId", aggregate.Id);
        command.Parameters.AddWithValue("AggregateType", aggregateType);
        command.Parameters.AddWithValue("ExpectedVersion", aggregate.Version);
        command.Parameters.AddWithValue("Events", NpgsqlDbType.Json, JsonSerializer.Serialize(eventDescriptors));
        command.Connection = (NpgsqlConnection)connection;

        var result = await command.ExecuteReaderAsync();
    }
}
