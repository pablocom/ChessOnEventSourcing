namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class WhenMovingPawns
{
    [Fact]
    public void TheyCanMove2ColumnsIfTheyAreInTheirInitialPosition()
    {
        var chessboard = new Chessboard(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now);

        chessboard.Move(from: Position.At(Column.D, Row.Two), to: Position.At(Column.D, Row.Four));
    }
}