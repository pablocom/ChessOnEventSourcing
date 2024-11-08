using ChessOnEventSourcing.Domain.PieceMoveStrategies.Specifications;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.PieceMoveStrategies;

public static class MoveStrategyFactory
{
    private static readonly (IMoveStrategySpecification Specification, Func<Chessboard, Square, Square, IMoveStrategy> Strategy)[] Strategies =
    [
        (new ShortCastleMoveStrategySpecification(), (chessboard, origin, dest) => new ShortCastleMoveStrategy(chessboard)),
        (new LongCastleMoveStrategySpecification(), (chessboard, origin, dest) => new LongCastleMoveStrategy(chessboard)),
        (new EnPassantMoveStrategySpecification(), (chessboard, origin, dest) => new EnPassantMoveStrategy(chessboard, origin, dest)),
        (new NormalMoveStrategySpecification(), (chessboard, origin, dest) => new NormalMoveStrategy(chessboard, origin, dest)),
    ];

    public static IMoveStrategy CreateMoveStrategy(Chessboard chessboard, Square origin, Square destination)
    {
        foreach (var (specification, strategy) in Strategies)
        {
            if (specification.IsSatisfiedBy(chessboard, origin, destination))
                return strategy(chessboard, origin, destination);
        }
        
        throw new ArgumentOutOfRangeException($"No strategy found for move {origin} -> {destination}");
    }

    public static PawnPromotionMoveStrategy CreateMoveStrategy(Chessboard chessboard, Square origin, Square destination, PieceType pieceType)
    {
        return new PawnPromotionMoveStrategy(chessboard, origin, destination, pieceType);
    }
}