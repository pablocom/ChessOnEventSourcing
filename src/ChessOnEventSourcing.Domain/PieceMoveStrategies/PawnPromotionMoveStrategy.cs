using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class PawnPromotionMoveStrategy : IPieceMoveStrategy
{
    private readonly PieceType _pieceType;

    public PawnPromotionMoveStrategy(PieceType pieceType)
    {
        _pieceType = pieceType;
    }
    
    public bool IsApplicable(Chessboard chessboard, Square origin, Square destination)
    {
        throw new NotImplementedException();
    }

    public bool IsValidMove(Chessboard chessboard, Square origin, Square destination)
    {
        throw new NotImplementedException();
    }

    public void Execute(Chessboard chessboard, Square origin, Square destination)
    {
        throw new NotImplementedException();
    }
}