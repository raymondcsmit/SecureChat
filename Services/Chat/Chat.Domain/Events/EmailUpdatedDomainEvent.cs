using MediatR;

namespace Chat.Domain.Events
{
    public class EmailUpdatedDomainEvent: INotification
    {
        public string Id { get; }
        public string NewEmail { get; }

        public EmailUpdatedDomainEvent(string id, string newEmail)
        {
            Id = id;
            NewEmail = newEmail;
        }
    }
}
