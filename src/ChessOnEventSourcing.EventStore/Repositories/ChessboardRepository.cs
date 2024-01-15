using ChessOnEventSourcing.Domain;

namespace ChessOnEventSourcing.EventStore.Repositories;

public sealed class ChessboardRepository : IChessboardRepository
{
    private readonly IEventStore _eventStore;

    public ChessboardRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Save(Chessboard chessboard) => await _eventStore.Save(chessboard);

    public async Task<Chessboard?> GetBy(Guid chessboardId)
    {
        var eventStream = await _eventStore.GetEventStream(chessboardId);

        Chessboard? chessboard = null;
        foreach (var @event in eventStream)
        {
            var deserializedEvent = EventDeserializer.Deserialize(@event);
            if (deserializedEvent is ChessboardCreated created)
            {
                chessboard = Chessboard.From(created);
                continue;
            }

            if (chessboard is null)
                throw new Exception($"Aggregate of type {@event.AggregateType} with id {@event.AggregateId} is missing it's creation event");

            chessboard.Apply(deserializedEvent);
        }

        chessboard?.ClearEvents();
        return chessboard;
    }
}
