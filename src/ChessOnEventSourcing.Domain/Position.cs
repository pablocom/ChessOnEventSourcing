namespace ChessOnEventSourcing.Domain;

public sealed class Position : IEquatable<Position>
{
    public Column Column { get; } = Column.H;
    public Row Row { get; } = Row.One;

    public Position(Column column, Row row)
    {
        Column = column;
        Row = row;
    }

    public static Position At(Column column, Row row)
    {
        return new Position(column, row);
    }

    public static bool operator ==(Position left, Position right) => left.Equals(right);
    public static bool operator !=(Position left, Position right) => !left.Equals(right);


    public bool Equals(Position other)
    {
        return Column == other.Column && Row == other.Row;
    }

    public override bool Equals(object? obj) => obj is Position && Equals((Position)obj);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Column.GetHashCode();
            hash = hash * 23 + Row.GetHashCode();
            return hash;
        }
    }
}

{
    public static readonly Row One = new(1);
    public static readonly Row Two = new(2);
    public static readonly Row Three = new(3);
    public static readonly Row Four = new(4);
    public static readonly Row Five = new(5);
    public static readonly Row Six = new(6);
    public static readonly Row Seven = new(7);
    public static readonly Row Eight = new(8);

    public static readonly IReadOnlyList<Row> All = new []{ One, Two, Three, Four, Five, Six, Seven, Eight };

    public int Value { get; }

    {
        Value = row;
    }

    public static bool operator ==(Row left, Row right) => left.Equals(right);
    public static bool operator !=(Row left, Row right) => !left.Equals(right);

    {
        if (other is null)
            return 1;

        return Value.CompareTo(other.Value);
    }

    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        return obj is Row row && Equals(row);
    }

    public override int GetHashCode() => Value.GetHashCode();
}

{
    public static readonly Column A = new('A');
    public static readonly Column B = new('B');
    public static readonly Column C = new('C');
    public static readonly Column D = new('D');
    public static readonly Column E = new('E');
    public static readonly Column F = new('F');
    public static readonly Column G = new('G');
    public static readonly Column H = new('H');

    public static readonly IReadOnlyList<Column> All = new[] { A, B, C, D, E, F, G, H };

    public char Value { get; }

    public Column(char column)
    {
        Value = column;
    }

    public static bool operator ==(Column left, Column right) => left.Equals(right);
    public static bool operator !=(Column left, Column right) => !left.Equals(right);


    {
        if (other is null)
            return 1;

        return Value.CompareTo(other.Value);
    }

    {
        if (other is null) 
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        return Equals((Column)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();
}