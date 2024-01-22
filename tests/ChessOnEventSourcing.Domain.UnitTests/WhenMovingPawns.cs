namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class WhenMovingPawns
{
    [Fact]
    public void TheyCanMove2ColumnsIfTheyAreInTheirInitialPosition()
    {
        var chessboard = new Chessboard(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now);

        chessboard.Move(origin: Position.At(Column.D, Row.Two), destination: Position.At(Column.D, Row.Four));
    }
}