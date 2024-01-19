using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Repositories;
using Microsoft.Extensions.Configuration;

namespace ChessOnEventSourcing.EventStore.IntegrationTests;

[Collection(nameof(IntegrationTestCollectionDefinition))]
public sealed class WhenSavingChessboard
{
    private readonly NpgsqlConnectionFactory _connectionFactory;
    private readonly NpgsqlUnitOfWork _unitOfWork;

    public WhenSavingChessboard(IntegrationTestFixture fixture)
    {
        _connectionFactory = new NpgsqlConnectionFactory(fixture.Configuration.GetConnectionString("Database")!);
        _unitOfWork = new NpgsqlUnitOfWork(_connectionFactory);
    }

    [Fact]
    public async Task ItsStoredAndHydratedFromEvents()
    {
        var chessBoardId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new NpgsqlEventStore(_unitOfWork));

        await _unitOfWork.BeginTransaction();
        await repository.Save(new Chessboard(chessBoardId, createdBy, createdAt));
        await _unitOfWork.Commit();

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().NotBeNull();  
        storedChessboard!.Id.Should().Be(chessBoardId);
        storedChessboard.CreatedBy.Should().Be(createdBy);
        storedChessboard.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public async Task ItsNotStoredOnTransactionRollback()
    {
        var chessBoardId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new NpgsqlEventStore(_unitOfWork));

        await _unitOfWork.BeginTransaction();
        await repository.Save(new Chessboard(chessBoardId, createdBy, createdAt));
        await _unitOfWork.Rollback();

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().BeNull();
    }

    [Fact]
    public async Task RetrievesCurrentTransactionAggregate()
    {
        var chessBoardId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new NpgsqlEventStore(_unitOfWork));

        await _unitOfWork.BeginTransaction();
        await repository.Save(new Chessboard(chessBoardId, createdBy, createdAt));

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().NotBeNull();
        storedChessboard!.Id.Should().Be(chessBoardId);
        storedChessboard.CreatedBy.Should().Be(createdBy);
        storedChessboard.CreatedAt.Should().Be(createdAt);
    }
}