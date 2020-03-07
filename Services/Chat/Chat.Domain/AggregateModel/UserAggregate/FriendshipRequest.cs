using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.Events;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class FriendshipRequest : Entity, IAuditable
    {
        public class Outcomes
        {
            public const string Accepted = "accepted";
            public const string Rejected = "rejected";
        }

        public string RequesterId { get; private set; }

        public string RequesteeId { get; private set; }

        public string Outcome { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset ModifiedAt { get; private set; }

        private FriendshipRequest() { }

        public FriendshipRequest(string requesterId, string requesteeId)
        {
            if (requesteeId == RequesterId)
            {
                throw new ChatDomainException("Requester id and requestee id cannot be equal");
            }

            RequesterId = requesterId;
            RequesteeId = requesteeId;
        }

        public void Accept()
        {
            if (Outcome != default)
            {
                throw new ChatDomainException($"Friendship request has already been {Outcome}");
            }

            Outcome = Outcomes.Accepted;
            AddDomainEvent(new FriendshipRequestAcceptedDomainEvent(this));
        }

        public void Reject()
        {
            if (Outcome != null)
            {
                throw new ChatDomainException($"Friendship request has already been {Outcome}");
            }

            Outcome = Outcomes.Rejected;
        }

        public bool IsPending => Outcome == null;
    }
}
