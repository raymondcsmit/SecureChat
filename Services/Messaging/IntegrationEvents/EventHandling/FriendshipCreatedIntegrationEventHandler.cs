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
    public class FriendshipCreatedIntegrationEventHandler : IIntegrationEventHandler<FriendshipCreatedIntegrationEvent>
    {
        private IHubContext<MessagingHub, IChatClient> _hubContext;
        private ILogger<FriendshipCreatedIntegrationEventHandler> _logger;

        public FriendshipCreatedIntegrationEventHandler(
            IHubContext<MessagingHub, IChatClient> hubContext,
            ILogger<FriendshipCreatedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(FriendshipCreatedIntegrationEvent @event, bool redelivered)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            await _hubContext.Clients
                .Groups(@event.Friendship.User1.Id, @event.Friendship.User2.Id)
                .FriendshipCreated(@event.Friendship);
        }
    }
}
