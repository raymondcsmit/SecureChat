using System.Collections.Generic;
using SecureChat.Common.Events.EventBus.Events;

namespace Users.API.Application.IntegrationEvents.Events
{
    public class NotifyUsersIntegrationEvent: IntegrationEvent
    {
        public IEnumerable<string> UserIds { get; }
        public string ClientMethod { get; }
        public object Data { get; }

        public NotifyUsersIntegrationEvent(IEnumerable<string> userIds, string clientMethod, object data = null)
        {
            UserIds = userIds;
            ClientMethod = clientMethod;
            Data = data;
        }
    }
}
