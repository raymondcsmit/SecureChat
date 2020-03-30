using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.Events;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Helpers.Mapping;

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
            if (Outcome == Outcomes.Accepted)
            {
                throw new ChatDomainException($"Friendship request has already been accepted");
            }

            Outcome = Outcomes.Accepted;
        }

        public void Reject()
        {
            if (!IsPending)
            {
                throw new ChatDomainException($"Friendship request has already been accepted or rejected");
            }

            Outcome = Outcomes.Rejected;
        }

        public bool IsPending => Outcome == null;
    }
}
