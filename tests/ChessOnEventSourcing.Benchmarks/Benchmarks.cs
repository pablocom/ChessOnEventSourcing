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
        var chessboard = Chessboard.Create(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now);
        
        chessboard.Move(Square.Parse("F2"), Square.Parse("F3"));
        chessboard.Move(Square.Parse("E7"), Square.Parse("E5"));
        
        chessboard.Move(Square.Parse("G2"), Square.Parse("G4"));
        chessboard.Move(Square.Parse("D8"), Square.Parse("H4"));
    }
}
