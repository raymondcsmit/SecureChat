using SecureChat.Common.Events.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Dtos;

namespace Users.API.Application.IntegrationEvents.Events
{
    public class FriendshipRequestCreatedIntegrationEvent : IntegrationEvent
    {
        public FriendshipRequestDto FriendshipRequest { get; }

        public FriendshipRequestCreatedIntegrationEvent(FriendshipRequestDto friendshipRequest)
        {
            FriendshipRequest = friendshipRequest;
        }
    }
}
