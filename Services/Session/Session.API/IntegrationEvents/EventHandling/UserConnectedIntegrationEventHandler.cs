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
    public class UserConnectedIntegrationEventHandler: IIntegrationEventHandler<UserConnectedIntegrationEvent>
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<UserConnectedIntegrationEventHandler> _logger;

        public UserConnectedIntegrationEventHandler(ISessionService sessionService, ILogger<UserConnectedIntegrationEventHandler> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task Handle(UserConnectedIntegrationEvent @event, bool redelivered)
        {
            _logger.LogInformation($"Creating session for user {@event.UserId}");
            await _sessionService.CreateSessionAsync(@event.UserId);
        }
    }
}
