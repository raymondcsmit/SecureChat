using MediatR;

namespace Chat.Domain.Events
{
    public class SessionEndedDomainEvent : INotification
    {
        public string UserId { get; }

        public SessionEndedDomainEvent(string userId)
        {
            UserId = userId;
        }
    }
}
