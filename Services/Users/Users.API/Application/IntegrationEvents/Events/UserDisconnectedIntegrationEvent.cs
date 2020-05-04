using SecureChat.Common.Events.EventBus.Events;

namespace Users.API.Application.IntegrationEvents.Events
{
    public class UserDisconnectedIntegrationEvent: IntegrationEvent
    {
        public string UserId { get; }

        public UserDisconnectedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
