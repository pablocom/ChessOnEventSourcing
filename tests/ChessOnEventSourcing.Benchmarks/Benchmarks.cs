using BenchmarkDotNet.Attributes;
using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class Benchmarks
{
    [Benchmark]
    public void FoolsMate()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.Now);

        chessboard.MovePiece(Square.Parse("F2"), Square.Parse("F3"));
        chessboard.MovePiece(Square.Parse("E7"), Square.Parse("E5"));
        
        chessboard.MovePiece(Square.Parse("G2"), Square.Parse("G4"));
        chessboard.MovePiece(Square.Parse("D8"), Square.Parse("H4"));
    }

    [Benchmark]
    public void ItalianGame()
    {
        var chessboard = Chessboard.Create(Guid.NewGuid(), DateTimeOffset.Now);

        chessboard.MovePiece(Square.Parse("E2"), Square.Parse("E4"));
        chessboard.MovePiece(Square.Parse("E7"), Square.Parse("E5"));

        chessboard.MovePiece(Square.Parse("G1"), Square.Parse("F3"));
        chessboard.MovePiece(Square.Parse("B8"), Square.Parse("C6"));

        chessboard.MovePiece(Square.Parse("F1"), Square.Parse("C4"));
        chessboard.MovePiece(Square.Parse("G8"), Square.Parse("F6"));
    }
}
