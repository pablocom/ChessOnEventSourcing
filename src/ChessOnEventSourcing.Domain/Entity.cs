namespace ChessOnEventSourcing.Domain;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected init; }

    public bool Equals(Entity? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        if (other.GetType() != GetType())
            return false;

        return Equals((Entity)other);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
