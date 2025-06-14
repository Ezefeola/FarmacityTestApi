using Core.Contracts.Models;

namespace Core.Domain.Abstractions;
public abstract class Entity<TId> : IEquatable<Entity<TId>>, IEntity<TId>
where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    protected Entity() { }
    protected Entity(TId id)
    {
        Id = id;
    }
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public DateTime FechaAlta { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Activo { get; set; }
}