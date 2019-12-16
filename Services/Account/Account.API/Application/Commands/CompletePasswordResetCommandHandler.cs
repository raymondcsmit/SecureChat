using System.Threading;
using System.Threading.Tasks;
using Account.API.Extensions;
using Account.API.Infrastructure.Exceptions;
using Account.API.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Account.API.Application.Commands
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
            var user = await _userManager.FindByIdAsync(notification.Id);
            if (user == null)
            {
                _logger.LogWarning($"Password reset failed (invalid id): {notification.Id}");
                throw new UsersApiException("Password reset failed", new[] { "User not found" }, 404);
            }

            var result = await _userManager.ResetPasswordAsync(user, notification.Token, notification.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Password reset for user id {user.Id}");
            }
            else
            {
                _logger.LogWarning($"Failed to reset password for user id {user.Id}: {result.Errors.ToErrorString()}");
                throw new UsersApiException("Password reset failed", result.Errors);
            }
        }
    }
}
