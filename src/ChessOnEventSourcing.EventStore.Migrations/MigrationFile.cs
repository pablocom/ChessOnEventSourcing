namespace ChessOnEventSourcing.EventStore.Migrations;

public sealed record MigrationFile(
    string Name, 
    string Content);