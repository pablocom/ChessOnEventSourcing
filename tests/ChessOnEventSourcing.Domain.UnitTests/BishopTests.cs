using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class BishopTests
{
    private readonly Dictionary<Square, Piece> _board = new();
    
    [Fact]
    public void CanMoveFreelyOnEmptyBoard()
    {
        var bishop = new Bishop(Square.Parse("d4"), Colour.White);
        
        _board.Add(bishop.Square, bishop);
        
        var expected = new[]
        {
            Square.At('E', 5), Square.At('F', 6), Square.At('G', 7), Square.At('H', 8),
            Square.At('E', 3), Square.At('F', 2), Square.At('G', 1),
            Square.At('C', 3), Square.At('B', 2), Square.At('A', 1),
            Square.At('C', 5), Square.At('B', 6), Square.At('A', 7)
        };

        var moves = bishop.GetAvailableMoves(_board);
        moves.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void IsBlockedBySameColorPiece()
    {
        var bishop = new Bishop(Square.At('A', 1), Colour.White);
        var blockingPiece = new Pawn(Square.At('B', 2), Colour.White);
        
        _board.Add(bishop.Square, bishop);
        _board.Add(blockingPiece.Square, blockingPiece);

        var moves = bishop.GetAvailableMoves(_board);
        
        moves.Should().BeEmpty();
    }

    [Fact]
    public void CanCaptureOpponentPiece()
    {
        var bishop = new Bishop(Square.At('A', 1), Colour.White);
        var opponentPieceSquare = Square.At('B', 2);
        var opponentPiece = new Bishop(opponentPieceSquare, Colour.Black);
        
        _board.Add(bishop.Square, bishop);
        _board.Add(opponentPiece.Square, opponentPiece);

        var moves = bishop.GetAvailableMoves(_board);
        
        moves.Should().HaveCount(1).And.BeEquivalentTo([opponentPieceSquare]);
    }
}