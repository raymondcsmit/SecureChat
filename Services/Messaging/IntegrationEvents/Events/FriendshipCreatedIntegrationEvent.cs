using Messaging.Dtos;
using SecureChat.Common.Events.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messaging.IntegrationEvents.Events
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
