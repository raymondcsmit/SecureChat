using SecureChat.Common.Events.EventBus.Events;

namespace Account.API.Application.IntegrationEvents.Events
{
    public class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public string UserName { get; }

        public string UserId { get;}

        public string Email { get; }

        public UserRegisteredIntegrationEvent(string userName, string userId, string email)
        {
            UserName = userName;
            UserId = userId;
            Email = email;
        }
    }
}
