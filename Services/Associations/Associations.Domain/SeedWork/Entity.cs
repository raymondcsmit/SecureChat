using System.Collections.Generic;
using MediatR;

namespace Associations.Domain.SeedWork
{
    public abstract class Entity
    {
        public string Id { get; protected set; }

        private List<INotification> _domainEvents = new List<INotification>();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public bool IsTransient => Id == default;

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity entity))
                return false;

            if (ReferenceEquals(this, entity))
                return true;

            if (GetType() != obj.GetType())
                return false;
            
            if (entity.IsTransient || IsTransient)
                return false;

            return entity.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient)
            {
                return Id.GetHashCode() ^ 31;
            }

            return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left?.Equals(right) ?? right?.Equals(left) ?? true;
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }

}
