using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Repositories;
using Microsoft.Extensions.Configuration;

namespace ChessOnEventSourcing.EventStore.IntegrationTests;

public sealed class IntegrationTestFixture
{
    public IConfigurationRoot Configuration { get; }

    public IntegrationTestFixture()
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
        Configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: false)
           .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();
    }
}


[CollectionDefinition(nameof(MappingIntegrationTestCollectionDefinition))]
public sealed class MappingIntegrationTestCollectionDefinition : ICollectionFixture<IntegrationTestFixture>;

[Collection(nameof(MappingIntegrationTestCollectionDefinition))]
public class WhenSavingChessboard
{
    private readonly NpgsqlConnectionFactory _connectionFactory;
    private readonly UnitOfWork _unitOfWork;

    public WhenSavingChessboard(IntegrationTestFixture fixture)
    {
        _connectionFactory = new NpgsqlConnectionFactory(fixture.Configuration.GetConnectionString("Database")!);
        _unitOfWork = new UnitOfWork(_connectionFactory);
    }

    [Fact]
    public async Task ItsStoredAndHydratedFromEvents()
    {
        var chessBoardId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new EventStore(_unitOfWork, _connectionFactory));

        await _unitOfWork.BeginTransaction();
        await repository.Save(new Chessboard(chessBoardId, createdBy, createdAt));
        await _unitOfWork.Commit();

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().NotBeNull();  
        storedChessboard!.Id.Should().Be(chessBoardId);
        storedChessboard.CreatedBy.Should().Be(createdBy);
        storedChessboard.CreatedAt.Should().Be(createdAt);
    }
}