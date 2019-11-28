using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Users.API.Models;
using Users.API.Services.Email;

namespace Users.API.Application.Commands
{
    public class ResetPasswordCommandHandler : INotificationHandler<ResetPasswordCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailGenerator _emailGenerator;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            IEmailSender emailSender,
            IEmailGenerator emailGenerator,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _emailGenerator = emailGenerator;
            _logger = logger;
        }

        public async Task Handle(ResetPasswordCommand notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initiating password reset for {notification.UserName}");
            var user = await _userManager.FindByNameAsync(notification.UserName);
            if (user == null)
            {
                _logger.LogWarning($"Failed password reset attempt (user does not exist): {notification.UserName}");
                return;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var (subject, body) =
                _emailGenerator.GeneratePasswordResetEmail(user.UserName, token, notification.CompletionUrl);
            await _emailSender.SendEmailAsync(user.Email, subject, body);
        }
    }
}
