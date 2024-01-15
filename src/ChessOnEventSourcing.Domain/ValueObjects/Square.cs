namespace ChessOnEventSourcing.Domain.ValueObjects;

public readonly struct Square : IEquatable<Square>
{
    public Column Column { get; }
    public Row Row { get; }

    private Square(Column column, Row row)
    {
        Column = column;
        Row = row;
    }

    public Square Add(int columns, int rows) => new(Column.Add(columns), Row.Add(rows));
    public Square AddRows(int rows) => new(Column, Row.Add(rows));
    public Square AddColumns(int columns) => new(Column.Add(columns), Row);

    public static Square At(Column column, Row row) => new(column, row);
    public static Square At(char column, int row) => new(Column.From(column), Row.From(row));
    
    public bool WouldOverflowAddingColumn(int columns) => Column.WouldOverflowAdding(columns);
    public bool WouldOverflowAddingRow(int rows) => Row.WouldOverflowAdding(rows);
    public bool WouldOverflowAdding(int columns, int rows) => Column.WouldOverflowAdding(columns) || Row.WouldOverflowAdding(rows);

    public static Square Parse(ReadOnlySpan<char> span)
    {
        if (span.Length != 2)
            throw new ArgumentException("Position can only be parsed from 2 length strings with format \"{Column}{Row}\"");

        return new Square(Column.From(span[0]), Row.From(span[1] - '0'));
    }

    public bool Equals(Square other) => Column == other.Column && Row == other.Row;

    public override int GetHashCode() => HashCode.Combine(Column, Row);

    public override string ToString() => $"{Column.Value}{Row.Value}";

    public override bool Equals(object? obj)
    {
        if (obj is Square square)
            return Equals(square);
        
        return false;
    }

    public static bool operator ==(Square left, Square right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Square left, Square right)
    {
        return !(left == right);
    }
}