using SecureChat.Common.Events.EventBus.Events;

namespace Chat.API.Application.IntegrationEvents.Events
{
    public class UserAccountCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserName { get;}

        public string UserId { get; }

        public string Email { get; }

        public UserAccountCreatedIntegrationEvent(string userName, string userId, string email)
        {
            UserName = userName;
            UserId = userId;
            Email = email;
        }
    }
}
