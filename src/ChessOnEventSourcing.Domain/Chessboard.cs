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

        AddEvent(new ChessboardCreated(Id, CreatedBy, CreatedAt));
    }

    public void Finish()
    {
        FinishedAt = DateTimeOffset.Now;
        
        AddEvent(new ChessboardFinished(Id, FinishedAt.Value));
    }
    
    public static Chessboard From(ChessboardCreated created)
    {
        var chessboard = new Chessboard(created.AggregateId, created.CreatedBy, created.CreatedAt);
        
        chessboard.ClearEvents();
        chessboard.Version = 1;
        return chessboard;
    }
    
    public override void Apply(Event @event)
    {
        switch (@event)
        {
            case ChessboardFinished finished:
                Apply(finished);
                break;
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
