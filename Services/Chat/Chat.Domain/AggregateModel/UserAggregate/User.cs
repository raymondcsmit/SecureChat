using Chat.Domain.Events;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        private string _userName;
        private string _email;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                AddDomainEvent(new UserNameUpdatedDomainEvent(Id, value));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                AddDomainEvent(new EmailUpdatedDomainEvent(Id, value));
            }
        }

        public Profile Profile { get; protected set; }

        public Session Session { get; protected set; }

        public bool ProfileCreated { get; protected set; }

        protected User() {}

        public User(string id, string userName, string email, Profile profile = null)
        {
            Id = id;
            _userName = userName;
            _email = email;
            Profile = profile;
        }

        public void EndSession()
        {
            Session = null;
            AddDomainEvent(new SessionEndedDomainEvent(Id));
        }

        public void RefreshSession()
        {
            if (Session == null)
            {
                throw new ChatDomainException("Could not refresh session",
                    new[] { "No session exists" });
            }

            Session.Refresh();
        }

        public void UpdateProfile(Profile profile)
        {
            if (Profile != profile)
            {
                Profile = profile;
                ProfileCreated = true;
            }
        }
    }
}
