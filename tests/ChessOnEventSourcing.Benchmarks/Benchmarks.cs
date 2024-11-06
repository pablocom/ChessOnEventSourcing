using BenchmarkDotNet.Attributes;
using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Benchmarks;

[MemoryDiagnoser]
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
}
