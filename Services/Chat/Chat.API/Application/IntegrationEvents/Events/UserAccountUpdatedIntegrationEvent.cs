using SecureChat.Common.Events.EventBus.Events;

namespace Chat.API.Application.IntegrationEvents.Events
{
    public class UserAccountUpdatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
