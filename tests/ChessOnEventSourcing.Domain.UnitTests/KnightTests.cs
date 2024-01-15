using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class KnightTests
{
    private readonly Dictionary<Square, Piece> _board = new();

    [Fact]
    public void CanMoveFreelyOnEmptyBoard()
    {
        var knight = new Knight(Square.At('D', 4), Colour.White);
        
        _board.Add(knight.Square, knight);

        var expected = new[]
        {
            Square.Parse("C6"), Square.Parse("E6"), Square.Parse("B5"), Square.Parse("F5"),
            Square.Parse("B3"), Square.Parse("F3"), Square.Parse("C2"), Square.Parse("E2")
        };

        var moves = knight.GetAvailableMoves(_board);
        moves.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void IsBlockedBySameColorPiece()
    {
        var knight = new Knight(Square.Parse("D4"), Colour.White);
        var blockingPiece = new Pawn(Square.Parse("C6"), Colour.White);
        
        _board.Add(knight.Square, knight);
        _board.Add(blockingPiece.Square, blockingPiece);
        
        var moves = knight.GetAvailableMoves(_board);
        moves.Should().NotContain(blockingPiece.Square);
    }
    
    [Fact]
    public void CanCaptureOpponentPiece()
    {
        var knight = new Knight(Square.Parse("D4"), Colour.White);
        var opponentPiece = new Pawn(Square.Parse("C6"), Colour.Black);
        
        _board.Add(knight.Square, knight);
        _board.Add(opponentPiece.Square, opponentPiece);

        var moves = knight.GetAvailableMoves(_board);
        moves.Should().Contain(opponentPiece.Square);
    }
}