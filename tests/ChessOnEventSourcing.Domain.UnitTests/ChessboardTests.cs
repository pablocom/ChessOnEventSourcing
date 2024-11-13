using ChessOnEventSourcing.Domain.Exceptions;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public sealed class ChessboardTests
{
    private readonly Chessboard _chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

    [Fact]
    public void CanMovePawnsTwoSquaresFromInitialPosition()
    {
        var origin = Square.Parse("D2");
        var destination = Square.Parse("D4");

        _chessboard.MovePiece(origin, destination);

        var pawn = _chessboard.GetPieceAt(destination);
        pawn.Type.Should().Be(PieceType.Pawn);
        pawn.Square.Should().Be(destination);
        pawn.Colour.Should().Be(Colour.White);
    }

    [Fact]
    public void CannotMovePawnTwoSquaresIfThereIsAPieceBlocking()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.White, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', 'p', 'p', ' ', 'p', 'p', 'p'],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'p', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', 'P', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', 'B', ' ', 'R']
        ]));

        var act = () => chessboard.MovePiece(Square.Parse("F2"), Square.Parse("F4"));

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void ThrowsIfMoveResultsInOwnKingBeingInCheck()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.White, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', ' ', 'p', 'p', 'p', 'p', 'p'],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            ['q', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', ' ', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', 'B', ' ', 'R']
        ]));

        var illegalMove = () => chessboard.MovePiece(Square.Parse("D2"), Square.Parse("D4"));

        illegalMove.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void ThrowsIfNoPieceWasFoundAtOriginSquare()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);
        
        var illegalMove = () => chessboard.MovePiece(Square.Parse("E3"), Square.Parse("E4"));

        illegalMove.Should().Throw<NoPieceFoundAtSquareException>()
            .WithMessage("No piece has been found at square E3");
    }

    [Fact]
    public void AllowsEnPassantCapture()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.Black, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', ' ', 'p', 'p', 'p', 'p', 'p'],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            ['q', ' ', ' ', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', ' ', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', 'B', ' ', 'R']
        ]));
        chessboard.MovePiece(Square.Parse("D7"), Square.Parse("D5"));

        chessboard.MovePiece(Square.Parse("E5"), Square.Parse("D6"));

        chessboard.GetPieceAt(Square.Parse("D6")).Type.Should().Be(PieceType.Pawn);
        chessboard.GetPieceAt(Square.Parse("D6")).Colour.Should().Be(Colour.White);
        chessboard.GetKilledPieces().Should().ContainSingle(p => p.Square.Equals(Square.Parse("D5")));
    }

    [Fact]
    public void DoesNotAllowEnPassantCaptureIfWouldCheckOwnKing()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.Black, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', ' ', 'p', 'q', 'p', 'p', 'p'],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            ['q', ' ', ' ', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', ' ', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', 'B', ' ', 'R']
        ]));
        chessboard.MovePiece(Square.Parse("D7"), Square.Parse("D5"));

        var act = () => chessboard.MovePiece(Square.Parse("E5"), Square.Parse("D6"));

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void AllowsShortCastlingForWhite()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.Black, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', ' ', 'p', ' ', 'p', 'p', 'p'],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'p', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', 'B', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', ' ', ' ', 'R']
        ]));
        
        chessboard.MovePiece(Square.Parse("D7"), Square.Parse("D5"));

        chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));

        chessboard.GetPieceAt(Square.Parse("G1")).Type.Should().Be(PieceType.King);
        chessboard.GetPieceAt(Square.Parse("F1")).Type.Should().Be(PieceType.Rook);
    }

    [Fact]
    public void AllowsShortCastlingForBlack()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.Black, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', ' ', ' ', 'r'],
            ['p', 'p', ' ', 'p', 'b', 'p', 'p', 'p'],
            [' ', ' ', ' ', ' ', 'p', 'n', ' ', ' '],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', 'B', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', 'P', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', ' ', ' ', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', ' ', ' ', 'R']
        ]));
        
        chessboard.MovePiece(Square.Parse("E8"), Square.Parse("G8"));

        chessboard.GetPieceAt(Square.Parse("G8")).Type.Should().Be(PieceType.King);
        chessboard.GetPieceAt(Square.Parse("F8")).Type.Should().Be(PieceType.Rook);
    }

    [Fact]
    public void DoesNotAllowShortCastlingIfKingHasMoved()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.White, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', ' ', 'p', ' ', 'p', 'p', 'p'],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'p', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', 'B', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', ' ', ' ', 'R']
        ]));
        chessboard.MovePiece(Square.Parse("E1"), Square.Parse("F1"));
        chessboard.MovePiece(Square.Parse("A7"), Square.Parse("A5"));
        chessboard.MovePiece(Square.Parse("F1"), Square.Parse("E1"));
        
        var act = () => chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void DoesNotAllowShortCastlingIfRookHasMovedAlready()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.White, MatrixToPiecesMapper.Map(
        [
            ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
            ['p', 'p', ' ', 'p', ' ', 'p', 'p', 'p'],
            [' ', ' ', 'p', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'p', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'P', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'N', ' ', ' '],
            ['P', 'P', 'P', 'P', 'B', 'P', 'P', 'P'],
            ['R', 'N', 'B', 'Q', 'K', ' ', ' ', 'R']
        ]));
        chessboard.MovePiece(Square.Parse("H1"), Square.Parse("G1"));
        chessboard.MovePiece(Square.Parse("A7"), Square.Parse("A5"));
        chessboard.MovePiece(Square.Parse("G1"), Square.Parse("H1"));
        
        var act = () => chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void DoesNotAllowToCastleIfWouldResultInCheck()
    {
        var pieces = MatrixToPiecesMapper.Map(
        [
            [' ', ' ', ' ', ' ', 'k', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'p', 'p', 'p'],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', 'b', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', 'P', 'P', 'P', 'P'],
            ['r', ' ', ' ', ' ', 'K', ' ', ' ', 'R']
        ]);
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.White, pieces);

        var act = () =>
        {
            chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));
        };

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void DoesNotAllowToCastleIfAnyPieceIsTargetingSquaresBetweenRookAndKing()
    {
        var pieces = MatrixToPiecesMapper.Map(
        [
            [' ', ' ', ' ', ' ', 'k', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', 'b', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', 'P', 'P', 'P'],
            [' ', ' ', ' ', ' ', 'K', ' ', ' ', 'R']
        ]);
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.UtcNow, Colour.White, pieces);

        var act = () =>
        {
            chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));
        };

        act.Should().Throw<InvalidMoveException>();
    }

    // TODO: does not allow to capture en passant if last move was not from pawn
    // TODO: does not allow to castle if any piece is targeting squares between root and king
    // TODO: does not allow to castle if is check
    
    [Fact]
    public void RecognizesFoolsMate()
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
    public void RecognizesScholarsMate()
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
}
