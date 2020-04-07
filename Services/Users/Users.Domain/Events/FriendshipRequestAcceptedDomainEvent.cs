using System;
using System.Collections.Generic;
using System.Text;
using Users.Domain.AggregateModel.UserAggregate;
using MediatR;

namespace Users.Domain.Events
{
    public class FriendshipRequestAcceptedDomainEvent: INotification
    {
        public FriendshipRequest FriendshipRequest { get; }

        public FriendshipRequestAcceptedDomainEvent(FriendshipRequest friendshipRequest)
        {
            FriendshipRequest = friendshipRequest;
        }
    }
}
