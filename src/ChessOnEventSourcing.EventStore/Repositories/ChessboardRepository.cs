using ChessOnEventSourcing.Domain;
using System.Text.Json;

namespace ChessOnEventSourcing.EventStore.Repositories;

public sealed class ChessboardRepository : IChessboardRepository
{
    private readonly IEventStore _eventStore;

    public ChessboardRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Chessboard?> GetBy(Guid chessboardId)
    {
        var events = await _eventStore.GetEvents(chessboardId);

        Chessboard? chessboard = null;

        foreach (var @event in events)
        {
            var deserializedEvent = DeserializeEvent(@event);

            if (deserializedEvent is ChessboardCreated chessBoardCreated)
            {
                chessboard = new Chessboard(chessBoardCreated.AggregateId, chessBoardCreated.CreatedBy, chessBoardCreated.CreatedAt);
                continue;
            }

            if (chessboard is null)
                throw new Exception($"Aggregate of type {@event.AggregateType} with id {@event.AggregateId} is missing it's creation event");

            chessboard.Apply(deserializedEvent);
        }

        return chessboard;
    }

    public async Task Save(Chessboard chessboard) => await _eventStore.Save(chessboard);

    private static DomainEvent DeserializeEvent(EventDescriptor eventDescriptor)
    {
        var eventType = Type.GetType(eventDescriptor.EventType);
        if (eventType is null)
            throw new Exception($"Could not find the corresponding CLR type for '{eventDescriptor.EventType}'");

        var deserializedEvent = (DomainEvent) JsonSerializer.Deserialize(eventDescriptor.EventData, eventType)!;
        return deserializedEvent;
    }
}
