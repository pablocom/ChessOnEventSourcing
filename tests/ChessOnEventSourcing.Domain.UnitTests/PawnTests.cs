using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class PawnTests
{
    private readonly Dictionary<Square, Piece> _board = new();

    [Fact]
    public void PawnCanMoveTwoSquaresFromInitialPosition()
    {
        var pawn = new Pawn(Square.At('E', 2), Colour.White);
        _board.Add(pawn.Square, pawn);
        
        var twoStepsForward = Square.At('E', 4);
        
        var moves = pawn.GetAvailableMoves(_board);
        
        moves.Should().Contain(twoStepsForward);
    }

    [Fact]
    public void PawnCanMoveTwoSquaresFromInitialPositionForBlackPieces()
    {
        var pawn = new Pawn(Square.At('E', 7), Colour.Black);
        _board.Add(pawn.Square, pawn);
        
        var twoStepsForward = Square.At('E', 5);
        
        var moves = pawn.GetAvailableMoves(_board);
        
        moves.Should().Contain(twoStepsForward);
    }
    
    [Fact]
    public void PawnCanMoveOneSquareForward()
    {
        var pawn = new Pawn(Square.At('e', 3), Colour.White);
        _board.Add(pawn.Square, pawn);

        var moves = pawn.GetAvailableMoves(_board);
        
        moves.Should().BeEquivalentTo([Square.At('E', 4)]);
    }
    
    [Fact]
    public void PawnCanMoveOneSquareForwardForBlackPieces()
    {
        var pawn = new Pawn(Square.At('E', 7), Colour.Black);
        var oneSquareForward = Square.At('E', 6);
        
        _board.Add(pawn.Square, pawn);

        var moves = pawn.GetAvailableMoves(_board);

        moves.Should().Contain(oneSquareForward);
    }

    [Fact]
    public void PawnCannotMoveForwardIfBlocked()
    {
        var pawn = new Pawn(Square.At('D', 4), Colour.White);
        var blockingPiece = new Pawn(Square.At('D', 5), Colour.White);
        _board.Add(pawn.Square, pawn);
        _board.Add(blockingPiece.Square, blockingPiece);

        var moves = pawn.GetAvailableMoves(_board);
        
        moves.Should().BeEmpty();
    }

    [Fact]
    public void PawnCanCaptureDiagonally()
    {
        var pawn = new Pawn(Square.At('D', 4), Colour.White);
        
        var captureLeft = Square.At('C', 5);
        var captureRight = Square.At('E', 5);
        var moveForward = Square.At('D', 5);
        
        var opponentPieceLeft = new Pawn(captureLeft, Colour.Black);
        var opponentPieceRight = new Pawn(captureRight, Colour.Black);
        _board.Add(pawn.Square, pawn);
        _board.Add(opponentPieceLeft.Square, opponentPieceLeft);
        _board.Add(opponentPieceRight.Square, opponentPieceRight);

        var moves = pawn.GetAvailableMoves(_board);
        
        moves.Should().BeEquivalentTo([captureLeft, captureRight, moveForward]);
    }
}