using System.Threading.Tasks;

namespace Account.API.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}