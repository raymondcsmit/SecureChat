using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SecureChat.Common.Events.EventBus.Events;

namespace Account.API.Application.IntegrationEvents.Events
{
    public class UserAccountUpdatedIntegrationEvent : IntegrationEvent
    {
        [JsonProperty]
        public string Id { get; private set; }

        [JsonProperty]
        public string UserName { get; private set; }

        [JsonProperty]
        public string Email { get; private set; }
    }
}
