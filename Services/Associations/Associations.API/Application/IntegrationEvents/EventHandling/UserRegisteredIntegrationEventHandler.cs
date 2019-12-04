using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Associations.API.Application.IntegrationEvents.Events;
using Associations.Domain.AggregateModel.UserAggregate;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Associations.API.Application.IntegrationEvents.EventHandling
{
    public class UserRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRegisteredIntegrationEventHandler> _logger;

        public UserRegisteredIntegrationEventHandler(
            IUserRepository userRepository,
            ILogger<UserRegisteredIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UserRegisteredIntegrationEvent @event, bool redelivered)
        {
            var existingUser = await _userRepository.GetAsync(@event.UserId);
            if (existingUser != null)
            {
                var result = _userRepository.Add(new User(@event.UserId, @event.UserName));
                if (result == null)
                {
                    _logger.LogError($"Could not add user with id {@event.UserId}");
                }
            }
        }
    }
}
