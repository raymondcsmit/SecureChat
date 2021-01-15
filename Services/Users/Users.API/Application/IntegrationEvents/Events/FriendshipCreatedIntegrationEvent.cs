using SecureChat.Common.Events.EventBus.Events;
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
