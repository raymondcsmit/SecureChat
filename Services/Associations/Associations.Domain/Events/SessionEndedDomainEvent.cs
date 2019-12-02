using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Associations.Domain.Events
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
