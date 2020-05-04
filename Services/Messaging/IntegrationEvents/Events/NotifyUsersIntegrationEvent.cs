using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SecureChat.Common.Events.EventBus.Events;

namespace Messaging.IntegrationEvents.Events
{
    public class NotifyUsersIntegrationEvent: IntegrationEvent
    {
        public IEnumerable<string> UserIds { get; }
        public string ClientMethod { get; }
        public JObject Data { get; }

        public NotifyUsersIntegrationEvent(IEnumerable<string> userIds, string clientMethod, JObject data = null)
        {
            UserIds = userIds;
            ClientMethod = clientMethod;
            Data = data;
        }
    }
}
