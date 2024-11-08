using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;
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
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new NpgsqlEventStore(_unitOfWork, _connectionFactory));

        await _unitOfWork.BeginTransaction();
        await repository.Save(Chessboard.Create(chessBoardId, createdAt));
        await _unitOfWork.Commit();

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().NotBeNull();  
        storedChessboard!.Id.Should().Be(chessBoardId);
        storedChessboard.CreatedAt.Should().Be(createdAt);
        storedChessboard.GetPieceAt(Square.Parse("E2")).Should().BeOfType<Pawn>();
        storedChessboard.GetPieceAt(Square.Parse("E1")).Should().BeOfType<King>();
        storedChessboard.Events.Should().BeEmpty();
    }

    [Fact]
    public async Task ItsNotStoredOnTransactionRollback()
    {
        var chessBoardId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new NpgsqlEventStore(_unitOfWork, _connectionFactory));

        await _unitOfWork.BeginTransaction();
        await repository.Save(Chessboard.Create(chessBoardId, createdAt));
        await _unitOfWork.Rollback();

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().BeNull();
    }

    [Fact]
    public async Task RetrievesCurrentTransactionAggregate()
    {
        var chessBoardId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new NpgsqlEventStore(_unitOfWork, _connectionFactory));

        await _unitOfWork.BeginTransaction();
        await repository.Save(Chessboard.Create(chessBoardId, createdAt));

        var storedChessboard = await repository.GetBy(chessBoardId);

        storedChessboard.Should().NotBeNull();
        storedChessboard!.Id.Should().Be(chessBoardId);
        storedChessboard.CreatedAt.Should().Be(createdAt);
    }
}