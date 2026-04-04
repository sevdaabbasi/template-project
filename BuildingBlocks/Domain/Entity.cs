using System.Collections.Generic;

namespace BuildingBlocks.Domain
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not Entity<TId> other)
                return false;

            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id!);
        }

        public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
        {
            return !(a == b);
        }
    }
}
