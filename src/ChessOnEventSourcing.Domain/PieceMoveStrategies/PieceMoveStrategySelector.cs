using ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public static class PieceMoveStrategySelector
{
    private static readonly (IMoveStrategySpecification Specification, Func<Chessboard, Square, Square, IPieceMoveStrategy> Strategy)[] Strategies =
    [
        (new ShortCastleMoveStrategySpecification(), (chessboard, origin, dest) => new ShortCastleMoveStrategy(chessboard, origin, dest)),
        (new LongCastleMoveStrategySpecification(), (chessboard, origin, dest) => new LongCastleMoveStrategy(chessboard, origin, dest)),
        (new EnPassantMoveStrategySpecification(), (chessboard, origin, dest) => new EnPassantMoveStrategy(chessboard, origin, dest)),
        (new NormalMoveStrategySpecification(), (chessboard, origin, dest) => new NormalPieceMoveStrategy(chessboard, origin, dest)),
    ];
    
    public static IPieceMoveStrategy GetMoveStrategy(Chessboard chessboard, Square origin, Square destination, PieceType? pieceType = null)
    {
        if (pieceType.HasValue)
        {
            if (PawnPromotionMoveStrategySpecification.IsApplicableTo(chessboard, origin, destination, pieceType.Value))
                return new PawnPromotionMoveStrategy(chessboard, origin, destination, pieceType.Value);
        }
        
        foreach (var (specification, strategy) in Strategies)
        {
            if (specification.IsApplicableTo(chessboard, origin, destination))
                return strategy(chessboard, origin, destination);
        }
        
        throw new ArgumentOutOfRangeException($"No strategy found for move {origin} -> {destination}");
    }
}