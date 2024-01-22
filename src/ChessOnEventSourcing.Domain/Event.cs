namespace ChessOnEventSourcing.Domain;

public abstract record Event(Guid AggregateId);
