using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Models;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using System.Data.Common;
using System.Text.Json;

namespace ChessOnEventSourcing.EventStore;

public sealed class NpgsqlEventStore : IEventStore
{
    private readonly IGetCurrentTransaction _currentTransactionProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public NpgsqlEventStore(IGetCurrentTransaction currentTransactionProvider, IDbConnectionFactory dbConnectionFactory)
    {
        _currentTransactionProvider = currentTransactionProvider;
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

        DbConnection? dbConnection;

        var transaction = _currentTransactionProvider.GetCurrentTransaction();
        if (transaction is null)
            dbConnection = await _dbConnectionFactory.CreateConnectionAsync(ct);
        else
            dbConnection = transaction.Connection;


        await using var command = dbConnection!.CreateCommand();

        command.CommandText = "CALL save_aggregate(@AggregateId, @AggregateType, @ExpectedVersion, @Events)";

        command.Parameters.Add(new NpgsqlParameter("AggregateId", aggregate.Id));
        command.Parameters.Add(new NpgsqlParameter("AggregateType", aggregateType));
        command.Parameters.Add(new NpgsqlParameter("ExpectedVersion", aggregate.Version));
        command.Parameters.Add(new NpgsqlParameter("Events", JsonSerializer.Serialize(eventDescriptors)) { NpgsqlDbType = NpgsqlDbType.Json });

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
