using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.Exceptions;

public class NoPieceFoundAtSquareException : Exception
{
    public NoPieceFoundAtSquareException(Square square) 
        : base($"No piece has been found at square {square}")
    { }
}