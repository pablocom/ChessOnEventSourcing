
using ChessOnEventSourcing.Domain.Events;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class WhenMovingPawns
{
    [Fact]
    public void TheyCanMove2ColumnsIfTheyAreInTheirInitialPosition()
    {
        var chessboard = new Chessboard(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now);

        chessboard.Move(Position.At(Column.D, Row.Two), Position.At(Column.D, Row.Four));

        chessboard.Events.Should().Contain(new PieceMoved(chessboard.Id, PieceType.Pawn, Column.D.Value, Row.Two.Value,
            Column.D.Value, Row.Four.Value));
    }
}