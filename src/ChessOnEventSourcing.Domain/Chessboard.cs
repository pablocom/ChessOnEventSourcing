namespace ChessOnEventSourcing.Domain;

public sealed class Chessboard : AggregateRoot
{
    public Guid CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? FinishedAt { get; private set; }

    public Chessboard(Guid id, Guid createdBy, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;

        AddDomainEvent(new ChessboardCreated(Id, CreatedBy, CreatedAt));
    }

    public override void Apply(DomainEvent @event)
    {
        switch (@event)
        {
            case ChessboardFinished finished:
                Apply(finished);
                break;
            default:
                throw new Exception($"No event handler found for event {@event.GetType().Name}");
        }

        Version++;
    }

    private void Apply(ChessboardFinished finished)
    {
        FinishedAt = finished.FinishedAt;
    }
}

public interface IChessboardRepository
{
    Task<Chessboard?> GetBy(Guid chessboardId);
    Task Save(Chessboard chessboard);
}
