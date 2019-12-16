﻿using SecureChat.Common.Events.EventBus.Events;

namespace Account.API.Application.IntegrationEvents.Events
{
    public class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }
    }
}
