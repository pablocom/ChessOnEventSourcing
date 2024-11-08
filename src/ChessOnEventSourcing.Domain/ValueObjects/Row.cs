namespace ChessOnEventSourcing.Domain.ValueObjects;

public readonly struct Row : IEquatable<Row>, IComparable<Row>
{
    public static readonly Row One = new(1);
    public static readonly Row Two = new(2);
    public static readonly Row Three = new(3);
    public static readonly Row Four = new(4);
    public static readonly Row Five = new(5);
    public static readonly Row Six = new(6);
    public static readonly Row Seven = new(7);
    public static readonly Row Eight = new(8);

    private static readonly Row[] All = [One, Two, Three, Four, Five, Six, Seven, Eight];
    
    public int Value { get; }

    private Row(int row)
    {
        if (row is < 1 or > 8)
            throw new ArgumentOutOfRangeException(nameof(row));
        
        Value = row;
    }

    public static Row From(int row)
    {
        if (row is < 1 or > 8)
            throw new ArgumentOutOfRangeException(nameof(row));

        return All[row - 1];
    }

    public Row Add(int offset) => All[Value - 1 + offset];

    public bool WouldOverflowAdding(int offset)
    {
        var index = Value - 1 + offset;
        return index is < 0 or > 7;
    }

    public static bool operator ==(Row left, Row right) => left.Equals(right);
    public static bool operator !=(Row left, Row right) => !left.Equals(right);

    public int CompareTo(Row other) => Value.CompareTo(other.Value);

    public bool Equals(Row other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is Row row)
            return Equals(row);

        return false;
    }

    public static bool operator <(Row left, Row right) => left.CompareTo(right) < 0;

    public static bool operator <=(Row left, Row right) => left.CompareTo(right) <= 0;

    public static bool operator >(Row left, Row right) => left.CompareTo(right) > 0;

    public static bool operator >=(Row left, Row right) => left.CompareTo(right) >= 0;
}