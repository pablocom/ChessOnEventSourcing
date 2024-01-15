using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class ChessboardTests
{
    [Fact]
    public void FoolsMate()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);

        chessboard.Move(Square.Parse("F2"), Square.Parse("F3"));
        chessboard.Move(Square.Parse("E7"), Square.Parse("E5"));
        
        chessboard.Move(Square.Parse("G2"), Square.Parse("G4"));
        chessboard.Move(Square.Parse("D8"), Square.Parse("H4"));

        chessboard.Winner.Should().NotBeNull();
        chessboard.Winner!.Should().Be(Colour.Black);
    }
    
    [Fact]
    public void ScholarsMate()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        
        chessboard.Move(Square.Parse("E2"), Square.Parse("E4"));
        chessboard.Move(Square.Parse("E7"), Square.Parse("E5"));
        
        chessboard.Move(Square.Parse("D1"), Square.Parse("H5"));
        chessboard.Move(Square.Parse("B8"), Square.Parse("C6"));

        chessboard.Move(Square.Parse("F1"), Square.Parse("C4"));
        chessboard.Move(Square.Parse("G8"), Square.Parse("F6"));

        chessboard.Move(Square.Parse("H5"), Square.Parse("F7"));

        chessboard.Winner.Should().NotBeNull();
        chessboard.Winner!.Should().Be(Colour.White);
    }
}