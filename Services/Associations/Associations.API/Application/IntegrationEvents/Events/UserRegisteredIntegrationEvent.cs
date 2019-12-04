using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SecureChat.Common.Events.EventBus.Events;

namespace Associations.API.Application.IntegrationEvents.Events
{
    public class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public string UserName { get; set; }

        public string UserId { get; set; }
    }
}
