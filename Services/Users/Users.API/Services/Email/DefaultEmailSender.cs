using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Users.API.Services.Email
{
    public class DefaultEmailSender : IEmailSender
    {
        public DefaultEmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        private EmailSenderOptions Options { get; }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            await Execute(Options.SendGridKey, subject, body, email);
        }

        private async Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.Sender, "Webmaster"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            await client.SendEmailAsync(msg);
        }
    }
}
