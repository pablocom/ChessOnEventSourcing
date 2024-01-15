namespace ChessOnEventSourcing.Domain;

public abstract class DomainEvent<TAggregateRootId> where TAggregateRootId : notnull
{
    public TAggregateRootId AggregateId { get; protected init; } = default!;
}
