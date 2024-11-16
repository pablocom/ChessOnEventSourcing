namespace ChessOnEventSourcing.Domain.ValueObjects;

public sealed class Colour : IEquatable<Colour>
{
    private readonly string _colour;

    private Colour(string colour)
    {
        _colour = colour;
    }
    
    public static readonly Colour White = new("White");
    public static readonly Colour Black = new("Black");

    public Colour Opposite() => Equals(White) ? Black : White;

    public bool Equals(Colour? other)
    {
        if (other is null) 
            return false;
        
        return _colour == other._colour;
    }

    public static bool operator ==(Colour left, Colour right) => left.Equals(right);
    public static bool operator !=(Colour left, Colour right) => !left.Equals(right);
    
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Colour other && Equals(other);

    public override int GetHashCode() => _colour.GetHashCode();

    public override string ToString() => _colour;
}
