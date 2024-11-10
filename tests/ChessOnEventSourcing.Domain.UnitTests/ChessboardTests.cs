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
        (Square Origin, Square Destination)[] sequence =
        [
            (Square.Parse("G1"), Square.Parse("F3")),
            (Square.Parse("E7"), Square.Parse("E5"))
        ];
        foreach (var move in sequence)
            _chessboard.MovePiece(move.Origin, move.Destination);

        var act = () => _chessboard.MovePiece(Square.Parse("F2"), Square.Parse("F4"));

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void IdentifiesFoolsMateCheckmate()
    {
        (Square Origin, Square Destination)[] foolsMateSequence =
        [
            (Square.Parse("F2"), Square.Parse("F3")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("G2"), Square.Parse("G4")),
            (Square.Parse("D8"), Square.Parse("H4"))
        ];

        foreach (var move in foolsMateSequence)
            _chessboard.MovePiece(move.Origin, move.Destination);

        _chessboard.FinishedAt.Should().NotBeNull();
        _chessboard.Winner.Should().Be(Colour.Black);
    }

    [Fact]
    public void ThrowsIfMoveResultsInOwnKingBeingInCheck()
    {
        (Square Origin, Square Destination)[] movesLeadingToVulnerableKing =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("D7"), Square.Parse("D5")),
            (Square.Parse("D1"), Square.Parse("H5"))
        ];

        foreach (var move in movesLeadingToVulnerableKing)
            _chessboard.MovePiece(move.Origin, move.Destination);

        var illegalMove = () => _chessboard.MovePiece(Square.Parse("F7"), Square.Parse("F6"));

        illegalMove.Should().Throw<InvalidMoveException>()
            .WithMessage("Illegal move");
    }

    [Fact]
    public void ThrowsIfNoPieceWasFoundAtOriginSquare()
    {
        var illegalMove = () => _chessboard.MovePiece(Square.Parse("E3"), Square.Parse("E4"));

        illegalMove.Should().Throw<NoPieceFoundAtSquareException>()
            .WithMessage($"No piece has been found at square E3");
    }

    [Fact]
    public void RecognizesScholarsMateCheckmate()
    {
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
            _chessboard.MovePiece(move.Origin, move.Destination);

        _chessboard.FinishedAt.Should().NotBeNull();
        _chessboard.Winner.Should().Be(Colour.White);
    }

    [Fact]
    public void AllowsEnPassantCapture()
    {
        (Square Origin, Square Destination)[] enPassantPreparationMoves =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("A7"), Square.Parse("A5")),
            (Square.Parse("E4"), Square.Parse("E5")),
            (Square.Parse("D7"), Square.Parse("D5"))
        ];
        foreach (var move in enPassantPreparationMoves)
            _chessboard.MovePiece(move.Origin, move.Destination);

        _chessboard.MovePiece(Square.Parse("E5"), Square.Parse("D6"));

        _chessboard.GetPieceAt(Square.Parse("D6")).Type.Should().Be(PieceType.Pawn);
        _chessboard.GetPieceAt(Square.Parse("D6")).Colour.Should().Be(Colour.White);
        _chessboard.GetKilledPieces().Should().ContainSingle(p => p.Square.Equals(Square.Parse("D5")));
    }

    [Fact]
    public void DoesNotAllowEnPassantCaptureIfWouldCheckOwnKing()
    {
        (Square Origin, Square Destination)[] enPassantPreparationMoves =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("A7"), Square.Parse("A5")),
            (Square.Parse("A2"), Square.Parse("A3")),
            (Square.Parse("A8"), Square.Parse("A6")),
            (Square.Parse("B2"), Square.Parse("B3")),
            (Square.Parse("A6"), Square.Parse("E6")),
            (Square.Parse("E4"), Square.Parse("E5")),
            (Square.Parse("D7"), Square.Parse("D5"))
        ];

        foreach (var move in enPassantPreparationMoves)
            _chessboard.MovePiece(move.Origin, move.Destination);

        var act = () => _chessboard.MovePiece(Square.Parse("E5"), Square.Parse("D6"));

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void AllowsShortCastlingForWhite()
    {
        var whiteKingSideCastling = (Square.Parse("E1"), Square.Parse("G1"));

        (Square Origin, Square Destination)[] kingSideCastlingMoves =
        [
            (Square.Parse("G2"), Square.Parse("G3")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("F1"), Square.Parse("G2")),
            (Square.Parse("D7"), Square.Parse("D5")),
            (Square.Parse("G1"), Square.Parse("F3")),
            (Square.Parse("C7"), Square.Parse("C5")),
            whiteKingSideCastling
        ];

        foreach (var move in kingSideCastlingMoves)
            _chessboard.MovePiece(move.Origin, move.Destination);

        _chessboard.GetPieceAt(Square.Parse("G1")).Type.Should().Be(PieceType.King);
        _chessboard.GetPieceAt(Square.Parse("F1")).Type.Should().Be(PieceType.Rook);
    }

    [Fact]
    public void AllowsShortCastlingForBlack()
    {
        (Square Origin, Square Destination)[] kingSideCastlingMoves =
        [
            (Square.Parse("D2"), Square.Parse("D4")),
            (Square.Parse("G7"), Square.Parse("G6")),
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("F8"), Square.Parse("G7")),
            (Square.Parse("E4"), Square.Parse("E5")),
            (Square.Parse("G8"), Square.Parse("F6")),
            (Square.Parse("C2"), Square.Parse("C4"))
        ];
        foreach (var move in kingSideCastlingMoves)
            _chessboard.MovePiece(move.Origin, move.Destination);

        _chessboard.MovePiece(Square.Parse("E8"), Square.Parse("G8"));

        _chessboard.GetPieceAt(Square.Parse("G8")).Type.Should().Be(PieceType.King);
        _chessboard.GetPieceAt(Square.Parse("F8")).Type.Should().Be(PieceType.Rook);
    }

    [Fact]
    public void DoesNotAllowShortCastlingIfKingHasMoved()
    {
        (Square Origin, Square Destination)[] moveSequence =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("G1"), Square.Parse("F3")),
            (Square.Parse("D7"), Square.Parse("D6")),
            (Square.Parse("G2"), Square.Parse("G3")),
            (Square.Parse("G8"), Square.Parse("F6")),
            (Square.Parse("F1"), Square.Parse("G2")),
            (Square.Parse("B7"), Square.Parse("B6")),
            (Square.Parse("E1"), Square.Parse("E2")),
            (Square.Parse("B6"), Square.Parse("B5")),
            (Square.Parse("E2"), Square.Parse("E1")),
            (Square.Parse("A7"), Square.Parse("A5"))
        ];
        foreach (var move in moveSequence)
            _chessboard.MovePiece(move.Origin, move.Destination);

        var act = () =>
        {
            _chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));
        };

        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void DoesNotAllowShortCastlingIfRookHasMovedAlready()
    {
        (Square Origin, Square Destination)[] moveSequence =
        [
            (Square.Parse("E2"), Square.Parse("E4")),
            (Square.Parse("E7"), Square.Parse("E5")),
            (Square.Parse("G1"), Square.Parse("F3")),
            (Square.Parse("D7"), Square.Parse("D6")),
            (Square.Parse("G2"), Square.Parse("G3")),
            (Square.Parse("G8"), Square.Parse("F6")),
            (Square.Parse("F1"), Square.Parse("G2")),
            (Square.Parse("B7"), Square.Parse("B6")),
            (Square.Parse("H1"), Square.Parse("G1")),
            (Square.Parse("B6"), Square.Parse("B5")),
            (Square.Parse("G1"), Square.Parse("H1")),
            (Square.Parse("A7"), Square.Parse("A5"))
        ];
        foreach (var move in moveSequence)
            _chessboard.MovePiece(move.Origin, move.Destination);

        var act = () => _chessboard.MovePiece(Square.Parse("E1"), Square.Parse("G1"));

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

    // TODO: does not allow to castle if any piece is targeting squares between root and king
    // TODO: does not allow to castle if is check
}
