using SecureChat.Common.Events.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Dtos;

namespace Users.API.Application.IntegrationEvents.Events
{
    public class FriendshipCreatedIntegrationEvent : IntegrationEvent
    {
        public FriendshipDto Friendship { get; }

        public FriendshipCreatedIntegrationEvent(FriendshipDto friendship)
        {
            Friendship = friendship;
        }
    }
}
