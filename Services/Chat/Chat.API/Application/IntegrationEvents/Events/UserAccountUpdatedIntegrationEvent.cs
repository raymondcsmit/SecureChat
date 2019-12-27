using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Domain.Events;
using Helpers.Mapping;
using SecureChat.Common.Events.EventBus.Events;

namespace Chat.API.Application.IntegrationEvents.Events
{
    public class UserAccountUpdatedIntegrationEvent : IntegrationEvent
    {
        public string Id { get; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public UserAccountUpdatedIntegrationEvent(string id)
        {
            Id = id;
        }
    }
}
