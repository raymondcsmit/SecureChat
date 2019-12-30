using Chat.Domain.Events;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public static class Flags
        {
            public const string ProfileAdded = nameof(ProfileAdded);
        }

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

        public Profile Profile { get; private set; }

        public Session Session { get; private set; }

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

        public void AddProfile(Profile profile)
        {
            if (Profile != profile)
            {
                Profile = profile;
                AddFlag(Flags.ProfileAdded);
            }
        }

        public bool HasProfile => Profile != null;
    }
}
