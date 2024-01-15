using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore.Repositories;

namespace ChessOnEventSourcing.EventStore.IntegrationTests;

public class WhenSavingChessboard
{
    [Fact]
    public async Task ItsStoredAndHydratedFromEvents()
    {
        var chessBoardId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2024, 1, 15, 17, 4, 0, TimeSpan.Zero);
        var repository = new ChessboardRepository(new EventStore());

        await repository.Save(new Chessboard(chessBoardId, createdBy, createdAt));

        var storedChessboard = await repository.GetBy(chessBoardId);
        storedChessboard.Should().NotBeNull();
        storedChessboard!.Id.Should().Be(chessBoardId);
        storedChessboard.CreatedBy.Should().Be(createdBy);
        storedChessboard.CreatedAt.Should().Be(createdAt);
    }
}