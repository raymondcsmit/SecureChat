using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Account.API.Extensions;
using Account.API.Infrastructure.Exceptions;
using Account.API.Models;
using Account.API.Services.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Account.API.Application.Commands
{
    public class ConfirmEmailCommandHandler: INotificationHandler<ConfirmEmailCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IEmailGenerator _emailGenerator;

        public ConfirmEmailCommandHandler(
            UserManager<User> userManager,
            ILogger<ConfirmEmailCommandHandler> logger,
            IEmailSender emailSender,
            IEmailGenerator emailGenerator)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _emailGenerator = emailGenerator;
        }

        public async Task Handle(ConfirmEmailCommand notification, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(notification.Id);
            if (user == null)
            {
                _logger.LogWarning($"Email confirmation failed (invalid id): {notification.Id}");
                throw new AccountApiException("Email confirmation failed", new[] { "User not found" }, 404);
            }

            if (notification.Token == null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var (subject, body) =
                    _emailGenerator.GenerateEmailConfirmationEmail(user.UserName, token);
                await _emailSender.SendEmailAsync(user.Email, subject, body);
                _logger.LogInformation($"Sending email confirmation email for user id {user.Id}");
                return;
            }


            var result = await _userManager.ConfirmEmailAsync(user, notification.Token);
            _logger.LogInformation(result.Succeeded
                ? $"Email confirmation succeeded for user id {user.Id}"
                : $"Email confirmation failed for user id {user.Id}: {result.Errors.ToErrorString()}");

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("email_verified", "true"));
                _logger.LogInformation($"Email confirmation succeeded for user id {user.Id}");
            }
            else
            {
                _logger.LogWarning($"Email confirmation failed for user id {user.Id}: {result.Errors.ToErrorString()}");
                throw new AccountApiException("Email confirmation failed", result.Errors);
            }
        }
    }
}
