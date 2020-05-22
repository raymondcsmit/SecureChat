using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;
using Session.API.IntegrationEvents.Events;
using Session.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.IntegrationEvents.EventHandling
{
    public class UserDisconnectedIntegrationEventHandler: IIntegrationEventHandler<UserDisconnectedIntegrationEvent>
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<UserDisconnectedIntegrationEventHandler> _logger;

        public UserDisconnectedIntegrationEventHandler(ISessionService sessionService, ILogger<UserDisconnectedIntegrationEventHandler> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task Handle(UserDisconnectedIntegrationEvent @event, bool redelivered)
        {
            _logger.LogInformation($"Ending session for user {@event.UserId}");
            await _sessionService.EndSessionAsync(@event.UserId);
        }
    }
}
