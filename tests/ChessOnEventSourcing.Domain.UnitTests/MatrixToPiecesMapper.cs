using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain.UnitTests;

public static class MatrixToPiecesMapper
{
    private const char EmptySquare = ' ';

    private static readonly Dictionary<char, Func<Square, Piece>> Mapping = new()
    {
        { 'K', square => new King(square, Colour.White) },
        { 'k', square => new King(square, Colour.Black) },
        { 'Q', square => new Queen(square, Colour.White) },
        { 'q', square => new Queen(square, Colour.Black) },
        { 'R', square => new Rook(square, Colour.White) },
        { 'r', square => new Rook(square, Colour.Black) },
        { 'B', square => new Bishop(square, Colour.White) },
        { 'b', square => new Bishop(square, Colour.Black) },
        { 'N', square => new Knight(square, Colour.White) },
        { 'n', square => new Knight(square, Colour.Black) },
        { 'P', square => new Pawn(square, Colour.White) },
        { 'p', square => new Pawn(square, Colour.Black) }
    };

    public static IReadOnlyDictionary<Square, Piece> Map(char[][] matrix)
    {
        var pieces = new Dictionary<Square, Piece>();

        for (var i = 0; i < matrix.Length; i++)
        {
            var row = Row.From(8 - i);
            for (var j = 0; j < matrix[i].Length; j++)
            {
                var column = Column.From(j + 1);
                var square = Square.At(column, row);

                var character = matrix[i][j];

                if (character is EmptySquare)
                    continue;

                if (!Mapping.TryGetValue(character, out var pieceFactory))
                    throw new InvalidOperationException($"Character '{character}' cannot be mapped into any piece");

                var piece = pieceFactory(square);
                pieces.Add(piece.Square, piece);
            }
        }

        return pieces.AsReadOnly();
    }
}

