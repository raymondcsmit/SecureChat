using Messaging.Clients;
using Messaging.Hubs;
using Messaging.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messaging.IntegrationEvents.EventHandling
{
    public class FriendshipRequestCreatedIntegrationEventHandler :
        IIntegrationEventHandler<FriendshipRequestCreatedIntegrationEvent>
    {
        private readonly IHubContext<MessagingHub, IChatClient> _hubContext;
        private readonly ILogger<FriendshipRequestCreatedIntegrationEventHandler> _logger;

        public FriendshipRequestCreatedIntegrationEventHandler(
            IHubContext<MessagingHub, IChatClient> hubContext,
            ILogger<FriendshipRequestCreatedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(FriendshipRequestCreatedIntegrationEvent @event, bool redelivered)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            await _hubContext.Clients
                .Group(@event.FriendshipRequest.Requestee.Id)
                .FriendshipRequestReceived(@event.FriendshipRequest);
        }
    }
}
