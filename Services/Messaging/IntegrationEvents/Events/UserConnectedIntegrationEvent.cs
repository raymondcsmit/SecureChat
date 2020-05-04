using SecureChat.Common.Events.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messaging.IntegrationEvents.Events
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
