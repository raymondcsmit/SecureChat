using SecureChat.Common.Events.EventBus.Events;

namespace Session.API.IntegrationEvents.Events
{
    public class UserConnectedIntegrationEvent: IntegrationEvent
    {
        public string UserId { get; }

        public UserConnectedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
