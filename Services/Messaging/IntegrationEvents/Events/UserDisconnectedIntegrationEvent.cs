using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureChat.Common.Events.EventBus.Events;

namespace Messaging.IntegrationEvents.Events
{
    public class UserDisconnectedIntegrationEvent: IntegrationEvent
    {
        public string UserId { get; }

        public UserDisconnectedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
