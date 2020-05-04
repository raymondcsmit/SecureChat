using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Hubs;
using Messaging.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Messaging.IntegrationEvents.EventHandling
{
    public class NotifyUsersIntegrationEventHandler: IIntegrationEventHandler<NotifyUsersIntegrationEvent>
    {
        private readonly IHubContext<MessagingHub> _hubContext;
        private readonly ILogger<NotifyUsersIntegrationEventHandler> _logger;

        public NotifyUsersIntegrationEventHandler(IHubContext<MessagingHub> hubContext, ILogger<NotifyUsersIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(NotifyUsersIntegrationEvent @event, bool redelivered)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            await _hubContext.Clients
                .Groups(@event.UserIds.ToList())
                .SendAsync(@event.ClientMethod, @event.Data.ToString());
        }
    }
}
