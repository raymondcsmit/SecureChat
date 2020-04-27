using Messaging.Dtos;
using SecureChat.Common.Events.EventBus.Events;

namespace Messaging.IntegrationEvents.Events
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
