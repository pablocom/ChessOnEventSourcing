using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public sealed class PawnPromotionMoveStrategy : IPieceMoveStrategy
{
    private readonly Chessboard _chessboard;
    private readonly Square _origin;
    private readonly Square _destination;
    private readonly PieceType _pieceType;

    public PawnPromotionMoveStrategy(Chessboard chessboard, Square origin, Square destination, PieceType pieceType)
    {
        _chessboard = chessboard;
        _origin = origin;
        _destination = destination;
        _pieceType = pieceType;
    }

    public bool IsValidMove()
    {
        throw new NotImplementedException();
    }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}