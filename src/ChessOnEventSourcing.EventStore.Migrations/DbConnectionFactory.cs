using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ChessOnEventSourcing.EventStore.Migrations;

public sealed class DbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<DbConnection> CreateDbConnection(CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        var dbConnection = new NpgsqlConnection(connectionString);

        await dbConnection.OpenAsync(cancellationToken);
        
        return dbConnection;
    }
}