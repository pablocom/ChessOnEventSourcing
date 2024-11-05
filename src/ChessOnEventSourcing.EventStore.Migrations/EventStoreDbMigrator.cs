using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ChessOnEventSourcing.EventStore.Migrations;

public sealed class EventStoreDbMigrator
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<EventStoreDbMigrator> _logger;

    public EventStoreDbMigrator(DbConnectionFactory dbConnectionFactory, ILogger<EventStoreDbMigrator> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }
    
    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        await using var dbConnection = await _dbConnectionFactory.CreateDbConnection(cancellationToken);

        await CreateMigrationsTableIfNotExists(dbConnection, cancellationToken);
        
        var lastAppliedMigrationName = await GetLastAppliedMigrationName(dbConnection, cancellationToken);
        var pendingMigrationFileNames = GetAllMigrationFileNames()
            .Where(x => string.Compare(x, lastAppliedMigrationName, StringComparison.InvariantCulture) > 0)
            .ToArray();
        
        if (pendingMigrationFileNames.Length is 0)
            _logger.LogInformation("No pending migrations to apply");
        
        await foreach (var migration in ReadMigrationFiles(pendingMigrationFileNames, cancellationToken))
        {
            await Apply(migration, dbConnection, cancellationToken);
        }
    }

    private static async Task CreateMigrationsTableIfNotExists(DbConnection dbConnection, CancellationToken cancellationToken)
    {
        await using var command = dbConnection.CreateCommand();

        command.CommandText = """
            CREATE TABLE IF NOT EXISTS "Migrations" (
              "Name" VARCHAR(255) NOT NULL PRIMARY KEY,
              "AppliedAt" TIMESTAMPTZ NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IDX_Migrations_AppliedAt ON "Migrations"("AppliedAt");
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    
    
    private static async Task<string?> GetLastAppliedMigrationName(DbConnection dbConnection, CancellationToken cancellationToken)
    {
        await using var command = dbConnection.CreateCommand();
        command.CommandText = """SELECT "Name" FROM "Migrations" ORDER BY "AppliedAt" DESC LIMIT 1;""";
        var result = await command.ExecuteScalarAsync(cancellationToken);
        
        return result as string;
    }
    
    private static string[] GetAllMigrationFileNames()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var migrationFilesDirectoryLocation = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, "Migrations");
        
        var migrationFileNames = Directory.GetFiles(migrationFilesDirectoryLocation)
            .Order(StringComparer.InvariantCultureIgnoreCase)
            .ToArray();
        return migrationFileNames;
    }
    
    private static async IAsyncEnumerable<MigrationFile> ReadMigrationFiles(string[] fileNames, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var fileName in fileNames)
        {
            var name = Path.GetFileName(fileName);
            var content = await File.ReadAllTextAsync(fileName, cancellationToken);

            yield return new MigrationFile(name, content);
        }
    }

    private async Task Apply(MigrationFile migration, DbConnection dbConnection, CancellationToken cancellationToken)
    {
        await using var transaction = await dbConnection.BeginTransactionAsync(cancellationToken);

        try
        {
            _logger.LogInformation("Applying migration {MigrationName}...", migration.Name);
            _logger.LogInformation("{MigrationName} content: \n {Content}", migration.Name, migration.Content);
        
            await ExecuteMigrationContent(migration, dbConnection, transaction, cancellationToken);
            await RecordMigrationApplied(migration, dbConnection, transaction, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Migration {MigrationName} applied...", migration.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying migration {MigrationName}", migration.Name);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task RecordMigrationApplied(MigrationFile migration, DbConnection dbConnection,
        DbTransaction transaction, CancellationToken cancellationToken)
    {
        await using var migrationsTableCommand = dbConnection.CreateCommand();
        
        migrationsTableCommand.Transaction = transaction;
        migrationsTableCommand.CommandText = """
            INSERT INTO "Migrations" ("Name", "AppliedAt") 
            VALUES (@Name, CURRENT_TIMESTAMP);
            """;
                
        var nameParameter = migrationsTableCommand.CreateParameter();
        nameParameter.ParameterName = "@Name";
        nameParameter.Value = migration.Name;
        migrationsTableCommand.Parameters.Add(nameParameter);
                
        await migrationsTableCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task ExecuteMigrationContent(MigrationFile migration, DbConnection dbConnection,
        DbTransaction transaction, CancellationToken cancellationToken)
    {
        await using var migrationContentCommand = dbConnection.CreateCommand();
        
        migrationContentCommand.CommandText = migration.Content;
        migrationContentCommand.Transaction = transaction;
        
        await migrationContentCommand.ExecuteNonQueryAsync(cancellationToken);
    }
}