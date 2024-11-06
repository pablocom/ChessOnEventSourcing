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
    private readonly IDbTransactionProvider _transactionProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public NpgsqlEventStore(IDbTransactionProvider transactionProvider, IDbConnectionFactory dbConnectionFactory)
    {
        _transactionProvider = transactionProvider;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Save(AggregateRoot aggregate, CancellationToken ct = default)
    {
        var aggregateType = aggregate.GetType().FullName!;
        var aggregateEvents = aggregate.Events.ToArray();

        var eventDescriptors = new List<EventDescriptor>(aggregateEvents.Length);
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

        var dbConnection = await GetDbConnection(ct);
        await using var command = dbConnection.CreateCommand();

        command.CommandText = "CALL save_aggregate(@AggregateId, @AggregateType, @ExpectedVersion, @Events)";

        command.Parameters.Add(new NpgsqlParameter("AggregateId", aggregate.Id));
        command.Parameters.Add(new NpgsqlParameter("AggregateType", aggregateType));
        command.Parameters.Add(new NpgsqlParameter("ExpectedVersion", aggregate.Version));
        command.Parameters.Add(new NpgsqlParameter("Events", JsonSerializer.Serialize(eventDescriptors)) { NpgsqlDbType = NpgsqlDbType.Json });

        await command.ExecuteNonQueryAsync(ct);
    }

    public Task<IEnumerable<EventDescriptor>> GetEventStream(Guid aggregateId, CancellationToken ct = default) 
        => GetEventsUntilDate(aggregateId, DateTimeOffset.MaxValue, ct);

    public async Task<IEnumerable<EventDescriptor>> GetEventsUntilDate(Guid aggregateId, DateTimeOffset date, CancellationToken ct = default)
    {
        var dbConnection = await GetDbConnection(ct);
        
        var result = await dbConnection.QueryAsync<EventDescriptor>("""
            SELECT "EventId", "AggregateId", "AggregateType", "EventType", "EventData", "Version", "OccurredOn" FROM "Events"
            WHERE "AggregateId" = @AggregateId AND "OccurredOn" <= @Date ORDER BY "Version" ASC;
            """,
            new { AggregateId = aggregateId, Date = date }
        );

        return result;
    }

    private async Task<DbConnection> GetDbConnection(CancellationToken ct)
    {
        var transaction = _transactionProvider.GetCurrentTransaction();

        return transaction is not null 
            ? transaction.Connection! 
            : await _dbConnectionFactory.CreateConnectionAsync(ct);
    }
}
