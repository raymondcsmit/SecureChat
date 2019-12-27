using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account.API.Application.Commands;
using Account.API.Application.IntegrationEvents.Events;
using Account.API.Infrastructure.Exceptions;
using Account.API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Account.API.Application.IntegrationEvents.EventHandling
{
    public class UserAccountUpdatedIntegrationEventHandler : IIntegrationEventHandler<UserAccountUpdatedIntegrationEvent>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserAccountUpdatedIntegrationEventHandler> _logger;

        public UserAccountUpdatedIntegrationEventHandler(
            UserManager<User> userManager,
            ILogger<UserAccountUpdatedIntegrationEventHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(UserAccountUpdatedIntegrationEvent @event, bool redelivered)
        {
            var user = await _userManager.FindByIdAsync(@event.Id);
            if (user == null)
            {
                _logger.LogWarning($"User update failed (invalid id): {@event.Id}");
                return;
            }

            if (@event.UserName != null)
            {
                user.UserName = @event.UserName;
            }

            if (@event.Email != null)
            {
                user.Email = @event.Email;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"User update failed: {@event.Id}");
                return;
            }
            _logger.LogInformation($"User update succeeded: {@event.Id}");
        }
    }
}
