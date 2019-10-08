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
    public class CompletePasswordResetCommandHandler : INotificationHandler<CompletePasswordResetCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CompletePasswordResetCommandHandler> _logger;

        public CompletePasswordResetCommandHandler(
            UserManager<User> userManager,
            ILogger<CompletePasswordResetCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(CompletePasswordResetCommand notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initiating password reset completion for {notification.UserName}");
            var user = await _userManager.FindByNameAsync(notification.UserName);
            if (user == null)
            {
                _logger.LogWarning($"Password reset failed (invalid id): {notification.UserName}");
                return;
            }

            var result = await _userManager.ResetPasswordAsync(user, notification.Token, notification.NewPassword);
            _logger.LogInformation(result.Succeeded
                ? $"Password reset for user id {user.Id}"
                : $"Failed to reset password for user id {user.Id}: {string.Join("\n", result.Errors)}");
        }
    }
}
