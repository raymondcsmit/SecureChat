using System.Threading.Tasks;
using Registration.Models;
using Registration.Views.Registration;

namespace Registration.Services
{
    public interface IUsersClient
    {
        Task<User> CreateUserAsync(RegistrationFormDto registrationFormDto);

        Task ConfirmEmailAsync(EmailConfirmationDto emailConfirmationDto);
        Task ResetPasswordAsync(string userName, string loginUrl);
        Task CompletePasswordResetAsync(string userName, string token, string password);
    }
}
