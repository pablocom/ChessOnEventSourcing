using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChessOnEventSourcing.EventStore.Migrations;

public sealed class EventStoreDbMigratorHostedService : IHostedLifecycleService
{
    private readonly EventStoreDbMigrator _eventStoreDbMigrator;
    private readonly ILogger<EventStoreDbMigratorHostedService> _logger;
    private readonly IHostApplicationLifetime _applicationHostLifetime;

    public EventStoreDbMigratorHostedService(
        EventStoreDbMigrator eventStoreDbMigrator, 
        ILogger<EventStoreDbMigratorHostedService> logger,
        IHostApplicationLifetime applicationHostLifetime)
    {
        _eventStoreDbMigrator = eventStoreDbMigrator;
        _logger = logger;
        _applicationHostLifetime = applicationHostLifetime;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        await _eventStoreDbMigrator.Migrate(cancellationToken);
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        _applicationHostLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}