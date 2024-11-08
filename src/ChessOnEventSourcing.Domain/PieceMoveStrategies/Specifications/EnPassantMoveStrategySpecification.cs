using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;

public sealed class EnPassantMoveStrategySpecification : IMoveStrategySpecification
{
    public bool IsSatisfiedBy(Chessboard chessboard, Square origin, Square destination)
    {
        var enPassantRow = chessboard.CurrentTurnColour == Colour.White ? Row.Five : Row.Four;
        
        var piece = chessboard.GetPieceAt(origin);
        
        if (piece.Type is not PieceType.Pawn) 
            return false;

        if (origin.Row != enPassantRow) 
            return false;
        
        return LastMoveWasEligiblePawnAdvanceForEnPassant(chessboard, enPassantRow) 
               && IsDiagonalMove(chessboard, origin, destination, enPassantRow);
    }
    
    private static bool LastMoveWasEligiblePawnAdvanceForEnPassant(Chessboard chessboard, Row enPassantRow)
    {
        if (chessboard.Moves.Count is 0)
            return false;
        
        var lastMove = chessboard.Moves[^1];
        
        if (lastMove.PieceType != PieceType.Pawn) 
            return false;
        
        var opponentPawnStartRow = chessboard.CurrentTurnColour == Colour.White ? Row.Seven : Row.Two;
        
        return lastMove.Origin.Row == opponentPawnStartRow && lastMove.Destination.Row == enPassantRow;
    }
    
    private static bool IsDiagonalMove(Chessboard chessboard, Square origin, Square destination, Row enPassantRow)
    {
        var pawnDirection = chessboard.CurrentTurnColour == Colour.White ? 1 : -1;

        var isDiagonalMove = (origin.Column == destination.Column.Add(1) || origin.Column == destination.Column.Add(-1))
                             && destination.Row == enPassantRow.Add(pawnDirection);
        return isDiagonalMove;
    }
}