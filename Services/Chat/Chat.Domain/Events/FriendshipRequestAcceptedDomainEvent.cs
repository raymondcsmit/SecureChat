using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.AggregateModel.UserAggregate;
using MediatR;

namespace Chat.Domain.Events
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
