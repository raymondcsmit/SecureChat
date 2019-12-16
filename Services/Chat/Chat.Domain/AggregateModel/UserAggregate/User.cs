using Chat.Domain.Events;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string UserName { get; protected set; }

        public Session Session { get; protected set; }

        protected User() {}

        public User(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public void EndSession()
        {
            Session = null;
            AddDomainEvent(new SessionEndedDomainEvent(Id));
        }

        public bool UpdateUsername(string userName)
        {
            if (UserName != userName)
            {
                UserName = userName;
                return true;
            }
            return false;
        }

        public void RefreshSession()
        {
            if (Session == null)
            {
                throw new AssociationsDomainException("Could not refresh session",
                    new[] { "No session exists" });
            }

            Session.Refresh();
        }


    }
}
