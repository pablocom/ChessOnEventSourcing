namespace ChessOnEventSourcing.Domain.ValueObjects;

public readonly struct Column : IEquatable<Column>, IComparable<Column>
{
    public static readonly Column A = new('A');
    public static readonly Column B = new('B');
    public static readonly Column C = new('C');
    public static readonly Column D = new('D');
    public static readonly Column E = new('E');
    public static readonly Column F = new('F');
    public static readonly Column G = new('G');
    public static readonly Column H = new('H');

    private static readonly Column[] _all = [A, B, C, D, E, F, G, H];
    public static IReadOnlyList<Column> All => _all.AsReadOnly();

    public char Value { get; }

    private Column(char column)
    {
        if (!IsInRange(column))
            throw new ArgumentOutOfRangeException(nameof(column));
        
        Value = char.IsAsciiLetterLower(column) ? char.ToUpper(column) : column;
    }

    public Column Add(int offset)
    {
        var newValue = (char)(Value + offset);
        if (!IsInRange(newValue))
            throw new ArgumentOutOfRangeException(nameof(offset));
        
        return new Column(newValue);
    }
    
    public bool WouldOverflowAdding(int offset)
    {
        var newValue = Value + offset;
        return newValue is < 'A' or > 'H';
    }
    
    public static Column From(char column) => new(column);

    private static bool IsInRange(char column) => column is >= 'A' and <= 'H' or >= 'a' and <= 'h';

    public static bool operator ==(Column left, Column right) => left.Equals(right);
    public static bool operator !=(Column left, Column right) => !left.Equals(right);

    public int CompareTo(Column other)
    {
        return Value.CompareTo(other.Value);
    }

    public bool Equals(Column other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Column other)
            return false;

        return Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static Column From(int number)
    {
        return number switch
        {
            1 => A,
            2 => B,
            3 => C,
            4 => D,
            5 => E,
            6 => F,
            7 => G,
            8 => H,
            _ => throw new ArgumentOutOfRangeException(nameof(number))
        };
    }

    public static bool operator <(Column left, Column right) => left.CompareTo(right) < 0;

    public static bool operator <=(Column left, Column right) => left.CompareTo(right) <= 0;

    public static bool operator >(Column left, Column right) => left.CompareTo(right) > 0;

    public static bool operator >=(Column left, Column right) => left.CompareTo(right) >= 0;
}