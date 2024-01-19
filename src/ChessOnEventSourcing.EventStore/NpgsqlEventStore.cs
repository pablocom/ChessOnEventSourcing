using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Models;
using Dapper;
using NpgsqlTypes;
using System.Text.Json;

namespace ChessOnEventSourcing.EventStore;

public sealed class NpgsqlEventStore : IEventStore
{
    private readonly IGetCurrentTransaction _currentTransactionProvider;

    public NpgsqlEventStore(IGetCurrentTransaction currentTransactionProvider)
    {
        _currentTransactionProvider = currentTransactionProvider;
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

        var transaction = _currentTransactionProvider.GetCurrentTransaction();
        await using var command = transaction.Connection!.CreateCommand();

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
        var result = await _currentTransactionProvider.GetCurrentTransaction().Connection!.QueryAsync<EventDescriptor>("""
            SELECT "EventId", "AggregateId", "AggregateType", "EventType", "EventData", "Version", "OccurredOn" FROM "Events"
            WHERE "AggregateId" = @AggregateId AND "OccurredOn" <= @Date ORDER BY "Version" ASC;
            """,
            new { AggregateId = aggregateId, Date = date }
        );

        return result;
    }
}
