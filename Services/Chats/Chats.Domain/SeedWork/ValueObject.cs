using System;
using System.Collections.Generic;
using System.Linq;

namespace Chats.Domain.SeedWork
{
    public abstract class ValueObject : ICloneable
    {
        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ValueObject other) || obj.GetType() != GetType())
            {
                return false;
            }

            var thisValues = GetAtomicValues().ToList();
            var otherValues = other.GetAtomicValues().ToList();
            return thisValues.Count == otherValues.Count && thisValues.Intersect(otherValues).Count() == thisValues.Count;
        }

        public override int GetHashCode()
        {
            return GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }
    }

}
