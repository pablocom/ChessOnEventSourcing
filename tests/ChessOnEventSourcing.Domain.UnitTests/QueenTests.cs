using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class QueenTests
{
    private readonly Dictionary<Square, Piece> _board = new();

    [Fact]
    public void CanMoveFreelyOnEmptyBoard()
    {
        var queen = new Queen(Square.At('D', 4), Colour.White);
        
        _board.Add(queen.Square, queen);

        var expected = new[]
        {
            Square.Parse("E4"), Square.Parse("F4"), Square.Parse("G4"), Square.Parse("H4"), 
            Square.Parse("C4"), Square.Parse("B4"), Square.Parse("A4"), Square.Parse("D5"), 
            Square.Parse("D6"), Square.Parse("D7"), Square.Parse("D8"), Square.Parse("D3"), 
            Square.Parse("D2"), Square.Parse("D1"), Square.Parse("C3"), Square.Parse("B2"), 
            Square.Parse("A1"), Square.Parse("E5"), Square.Parse("F6"), Square.Parse("G7"), 
            Square.Parse("H8"), Square.Parse("E3"), Square.Parse("F2"), Square.Parse("G1"), 
            Square.Parse("C5"), Square.Parse("B6"), Square.Parse("A7")
        };

        var moves = queen.GetAvailableMoves(_board);
        moves.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void IsBlockedBySameColorPiece()
    {
        var queen = new Queen(Square.Parse("A1"), Colour.White);
        var blockingPiece1 = new Pawn(Square.Parse("B2"), Colour.White);
        var blockingPiece2 = new Pawn(Square.Parse("A2"), Colour.White);
        var blockingPiece3 = new Rook(Square.Parse("b1"), Colour.White);
        
        _board.Add(queen.Square, queen);
        _board.Add(blockingPiece1.Square, blockingPiece1);
        _board.Add(blockingPiece2.Square, blockingPiece2);
        _board.Add(blockingPiece3.Square, blockingPiece3);

        var moves = queen.GetAvailableMoves(_board);
        
        moves.Should().BeEmpty();
    }
    
    [Fact]
    public void CanCaptureOpponentPiece()
    {
        var queen = new Queen(Square.Parse("A1"), Colour.White);
        var opponentPiece = new Rook(Square.Parse("b1"), Colour.Black);
        var blockingPiece1 = new Pawn(Square.Parse("B2"), Colour.White);
        var blockingPiece2 = new Pawn(Square.Parse("A2"), Colour.White);
        
        _board.Add(queen.Square, queen);
        _board.Add(blockingPiece1.Square, blockingPiece1);
        _board.Add(blockingPiece2.Square, blockingPiece2);
        _board.Add(opponentPiece.Square, opponentPiece);

        var moves = queen.GetAvailableMoves(_board);
        
        moves.Should().HaveCount(1);
        moves.Single().Should().Be(opponentPiece.Square);
    }
}