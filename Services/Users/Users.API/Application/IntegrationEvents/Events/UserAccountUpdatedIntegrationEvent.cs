using SecureChat.Common.Events.EventBus.Events;

namespace Users.API.Application.IntegrationEvents.Events
{
    public class UserAccountUpdatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public UserAccountUpdatedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
