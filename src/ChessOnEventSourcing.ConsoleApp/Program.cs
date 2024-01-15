using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.Domain.ValueObjects;

var foolsMateChessboard = Chessboard.Create(Guid.NewGuid(), Guid.NewGuid(), TimeProvider.System.GetUtcNow());

foolsMateChessboard.Move(Square.Parse("E2"), Square.Parse("E4"));
foolsMateChessboard.Move(Square.Parse("E7"), Square.Parse("E5"));

foolsMateChessboard.Move(Square.Parse("D1"), Square.Parse("H5"));
foolsMateChessboard.Move(Square.Parse("B8"), Square.Parse("C6"));

foolsMateChessboard.Move(Square.Parse("F1"), Square.Parse("C4"));
foolsMateChessboard.Move(Square.Parse("G8"), Square.Parse("F6"));

foolsMateChessboard.Move(Square.Parse("H5"), Square.Parse("F7"));

