namespace ChessOnEventSourcing.Domain;

public abstract record DomainEvent(Guid AggregateId);
