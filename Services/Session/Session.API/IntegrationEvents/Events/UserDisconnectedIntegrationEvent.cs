using SecureChat.Common.Events.EventBus.Events;

namespace Session.API.IntegrationEvents.Events
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
