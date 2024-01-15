namespace ChessOnEventSourcing.Domain;

public abstract class DomainEvent
{
    public Guid AggregateId { get; protected init; } = default!;
}
