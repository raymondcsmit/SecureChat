using MediatR;

namespace Users.Domain.Events
{
    public class UserNameUpdatedDomainEvent: INotification
    {
        public string Id { get; }
        public string NewUserName { get; }

        public UserNameUpdatedDomainEvent(string id, string newUserName)
        {
            Id = id;
            NewUserName = newUserName;
        }
    }
}
