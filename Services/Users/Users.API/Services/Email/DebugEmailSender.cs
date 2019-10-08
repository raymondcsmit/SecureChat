using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Users.API.Services.Email
{
    public class DebugEmailSender : IEmailSender
    {
        private readonly ILogger<DebugEmailSender> _logger;

        public DebugEmailSender(ILogger<DebugEmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            _logger.LogDebug($"email: {email}, subject: {subject}, body: {body}");
        }
    }
}
