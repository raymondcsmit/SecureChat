using SecureChat.Common.Events.EventBus.Events;

namespace Users.API.Application.IntegrationEvents.Events
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
