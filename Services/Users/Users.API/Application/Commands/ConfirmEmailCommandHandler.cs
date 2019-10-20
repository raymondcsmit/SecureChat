using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Users.API.Extensions;
using Users.API.Infrastructure.Exceptions;
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
            var user = await _userManager.FindByIdAsync(notification.Id);
            if (user == null)
            {
                _logger.LogWarning($"Email confirmation failed (invalid id): {notification.Id}");
                throw new UsersApiException("Email confirmation failed", new[] { "User not found" }, 404);
            }

            var result = await _userManager.ConfirmEmailAsync(user, notification.Token);
            _logger.LogInformation(result.Succeeded
                ? $"Email confirmation succeeded for user id {user.Id}"
                : $"Email confirmation failed for user id {user.Id}: {result.Errors.ToErrorString()}");

            if (result.Succeeded)
            {
                _logger.LogInformation($"Email confirmation succeeded for user id {user.Id}");
            }
            else
            {
                _logger.LogWarning($"Email confirmation failed for user id {user.Id}: {result.Errors.ToErrorString()}");
                throw new UsersApiException("Email confirmation failed", result.Errors);
            }
        }
    }
}
