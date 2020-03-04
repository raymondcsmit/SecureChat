using System.Collections.Generic;
using System.Linq;
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
        private Profile _profile;

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

        public Profile Profile
        {
            get => _profile;
            set
            {
                if (_profile == null && value != null)
                {
                    AddFlag(Flags.ProfileAdded);
                }
                _profile = value;
            }
        }

        public Session Session { get; private set; }

        private List<FriendshipRequest> _friendshipRequests = new List<FriendshipRequest>();

        public IEnumerable<FriendshipRequest> PendingFriendshipRequests =>
            _friendshipRequests.Where(assoc =>
                assoc is FriendshipRequest req && req.RequesteeId == Id && req.IsPending);

        public IEnumerable<FriendshipRequest> MyPendingFriendshipRequests =>
            _friendshipRequests.Where(assoc =>
                assoc is FriendshipRequest req && req.RequesterId == Id && req.IsPending);

        public User(string id, string userName, string email, Profile profile = null, IEnumerable<FriendshipRequest> friendshipRequests = null)
        {
            Id = id;
            _userName = userName;
            _email = email;
            _profile = profile;
            if (friendshipRequests != null)
            {
                _friendshipRequests.AddRange(friendshipRequests);
            }
        }

        //public void EndSession()
        //{
        //    Session = null;
        //    AddDomainEvent(new SessionEndedDomainEvent(Id));
        //}

        //public void RefreshSession()
        //{
        //    if (Session == null)
        //    {
        //        throw new ChatDomainException("Could not refresh session",
        //            new[] { "No session exists" });
        //    }

        //    Session.Refresh();
        //}

        public bool HasProfile => Profile != null;

        public void MakeFriendshipRequest(User user)
        {
            if (_friendshipRequests.Any(req => req.RequesterId == user.Id || req.RequesteeId == user.Id))
            {
                throw new ChatDomainException("Friendship request already exists");
            }

            var friendshipRequest = new FriendshipRequest(Id, user.Id);
            _friendshipRequests.Add(new FriendshipRequest(Id, user.Id));
        }
    }
}
