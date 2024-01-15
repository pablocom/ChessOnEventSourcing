namespace ChessOnEventSourcing.Domain;

public sealed class ChessboardCreated : DomainEvent
{
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ChessboardCreated(Guid chessboardId, Guid createdBy, DateTimeOffset createdAt)
    {
        AggregateId = chessboardId;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }
}