using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.Domain.ValueObjects;

var foolsMateChessboard = Chessboard.Create(Guid.NewGuid(), TimeProvider.System.GetUtcNow());

foolsMateChessboard.MovePiece(Square.Parse("E2"), Square.Parse("E4"));
foolsMateChessboard.MovePiece(Square.Parse("E7"), Square.Parse("E5"));

foolsMateChessboard.MovePiece(Square.Parse("D1"), Square.Parse("H5"));
foolsMateChessboard.MovePiece(Square.Parse("B8"), Square.Parse("C6"));

foolsMateChessboard.MovePiece(Square.Parse("F1"), Square.Parse("C4"));
foolsMateChessboard.MovePiece(Square.Parse("G8"), Square.Parse("F6"));

foolsMateChessboard.MovePiece(Square.Parse("H5"), Square.Parse("F7"));

