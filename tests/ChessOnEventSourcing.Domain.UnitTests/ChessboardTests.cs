using ChessOnEventSourcing.Domain.Exceptions;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class ChessboardTests
{
    [Fact]
    public void IdentifiesFoolsMateCheckmate()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        (Square Origin, Square Destination)[] foolsMateSequence =
        [
            (Square.Parse("F2"), Square.Parse("F3")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("G2"), Square.Parse("G4")),
            (Square.Parse("D8"), Square.Parse("H4"))
        ];

        foreach (var move in foolsMateSequence)
            chessboard.MovePiece(move.Origin, move.Destination);

        chessboard.FinishedAt.Should().NotBeNull();
        chessboard.Winner.Should().Be(Colour.Black);
    }

    [Fact]
    public void ThrowsIfMoveResultsInOwnKingBeingInCheck()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        (Square Origin, Square Destination)[] movesLeadingToVulnerableKing =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("D7"), Square.Parse("D5")),
            (Square.Parse("D1"), Square.Parse("H5"))
        ];

        foreach (var move in movesLeadingToVulnerableKing)
            chessboard.MovePiece(move.Origin, move.Destination);

        var illegalMove = () => chessboard.MovePiece(Square.Parse("F7"), Square.Parse("F6"));

        illegalMove.Should().Throw<InvalidMoveException>()
            .WithMessage("Illegal move");
    }
    
    [Fact]
    public void ThrowsIfNoPieceWasFoundAtSquare()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        var illegalMove = () => chessboard.MovePiece(Square.Parse("E3"), Square.Parse("E4"));

        illegalMove.Should().Throw<NoPieceFoundAtSquareException>()
            .WithMessage($"No piece has been found at square E3");
    }

    [Fact]
    public void RecognizesScholarsMateCheckmate()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        (Square Origin, Square Destination)[] scholarsMateSequence =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("D1"), Square.Parse("H5")),
            (Square.Parse("B8"), Square.Parse("C6")),
            (Square.Parse("F1"), Square.Parse("C4")),
            (Square.Parse("G8"), Square.Parse("F6")),
            (Square.Parse("H5"), Square.Parse("F7"))
        ];

        foreach (var move in scholarsMateSequence)
            chessboard.MovePiece(move.Origin, move.Destination);

        chessboard.FinishedAt.Should().NotBeNull();
        chessboard.Winner.Should().Be(Colour.White);
    }

    [Fact]
    public void AllowsEnPassantCapture()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        (Square Origin, Square Destination)[] enPassantPreparationMoves =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("A7"), Square.Parse("A5")),
            (Square.Parse("E4"), Square.Parse("E5")),
            (Square.Parse("D7"), Square.Parse("D5"))
        ];
        foreach (var move in enPassantPreparationMoves)
            chessboard.MovePiece(move.Origin, move.Destination);

        chessboard.MovePiece(Square.Parse("E5"), Square.Parse("D6"));
        
        chessboard.GetPieceAt(Square.Parse("D6")).Type.Should().Be(PieceType.Pawn);
        chessboard.GetPieceAt(Square.Parse("D6")).Colour.Should().Be(Colour.White);
        chessboard.GetKilledPieces().Should().ContainSingle(p => p.Square.Equals(Square.Parse("D5")));
    }

    [Fact]
    public void ExecutesKingSideCastlingCorrectly()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        var whiteKingSideCastling = (Square.Parse("E1"), Square.Parse("G1"));
        
        (Square Origin, Square Destination)[] kingSideCastlingMoves =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("G1"), Square.Parse("F3")),
            (Square.Parse("G8"), Square.Parse("F6")),
            (Square.Parse("F1"), Square.Parse("C4")),
            (Square.Parse("F8"), Square.Parse("C5")),
            whiteKingSideCastling
        ];

        foreach (var move in kingSideCastlingMoves)
            chessboard.MovePiece(move.Origin, move.Destination);

        chessboard.GetPieceAt(Square.Parse("G1")).Type.Should().Be(PieceType.King);
        chessboard.GetPieceAt(Square.Parse("F1")).Type.Should().Be(PieceType.Rook);
    }
}
