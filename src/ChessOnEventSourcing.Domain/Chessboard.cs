namespace ChessOnEventSourcing.Domain;

public sealed class Chessboard : AggregateRoot
{
    public Guid CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }

    public Chessboard(Guid id, Guid createdBy, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;

        AddDomainEvent(new ChessboardCreated(Id, CreatedBy, CreatedAt));
    }

    public override void Apply(DomainEvent @event)
    {
        Version++;
    }
}

public interface IChessboardRepository
{
    Task<Chessboard?> GetBy(Guid chessboardId);
    Task Save(Chessboard chessboard);
}