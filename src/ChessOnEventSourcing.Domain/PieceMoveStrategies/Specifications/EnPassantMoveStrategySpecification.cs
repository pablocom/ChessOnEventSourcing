using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class EnPassantMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsApplicableTo(Chessboard chessboard, Square origin, Square destination, PieceType? _ = null)
    {
        var enPassantRow = chessboard.CurrentTurnColour == Colour.White ? Row.Five : Row.Four;
        
        if (!chessboard.TryGetPieceAt(origin, out var piece))
            return false;
        
        if (piece.Type is not PieceType.Pawn) 
            return false;

        if (origin.Row != enPassantRow) 
            return false;
        
        var opponentPawnStartRow = chessboard.CurrentTurnColour == Colour.White ? Row.Seven : Row.Two;

        if (!chessboard.Moves.Any())
            return false;
        
        var lastMove = chessboard.Moves[^1];
        if (lastMove.PieceType != PieceType.Pawn || lastMove.Colour != chessboard.CurrentTurnColour.Opposite()) 
            return false;
        
        if (lastMove.Origin.Row != opponentPawnStartRow || lastMove.Destination.Row != enPassantRow) 
            return false;

        var pawnDirection = chessboard.CurrentTurnColour == Colour.White ? 1 : -1; 
        var isDiagonalMove = (origin.Column == destination.Column.Add(1) || origin.Column == destination.Column.Add(-1))
                             && destination.Row == enPassantRow.Add(pawnDirection);
        if (!isDiagonalMove) 
            return false;
        
        return !chessboard.TryGetPieceAt(destination, out var _);
    }
}