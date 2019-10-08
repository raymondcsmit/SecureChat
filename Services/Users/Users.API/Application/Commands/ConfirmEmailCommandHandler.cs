using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Users.API.Models;

namespace Users.API.Application.Commands
{
    public class ConfirmEmailCommandHandler: INotificationHandler<ConfirmEmailCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(
            UserManager<User> userManager,
            ILogger<ConfirmEmailCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(ConfirmEmailCommand notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initiating email confirmation for {notification.Id}");
            var user = await _userManager.FindByIdAsync(notification.Id);
            if (user == null)
            {
                _logger.LogWarning($"Emailed confirmation failed (invalid id): {notification.Id}");
                return;
            }

            var result = await _userManager.ConfirmEmailAsync(user, notification.Token);
            _logger.LogInformation(result.Succeeded
                ? $"Email confirmation succeeded for user id {user.Id}"
                : $"Email confirmation failed for user id {user.Id}: {string.Join("\n", result.Errors)}");
        }
    }
}
