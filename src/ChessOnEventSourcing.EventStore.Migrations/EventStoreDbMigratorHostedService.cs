using Microsoft.Extensions.Hosting;

namespace ChessOnEventSourcing.EventStore.Migrations;

public sealed class EventStoreDbMigratorHostedService : IHostedLifecycleService
{
    private readonly EventStoreDbMigrator _eventStoreDbMigrator;
    private readonly IHostApplicationLifetime _hostLifetime;

    public EventStoreDbMigratorHostedService(EventStoreDbMigrator eventStoreDbMigrator, IHostApplicationLifetime hostLifetime)
    {
        _eventStoreDbMigrator = eventStoreDbMigrator;
        _hostLifetime = hostLifetime;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken) => await _eventStoreDbMigrator.Migrate(cancellationToken);

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        _hostLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}