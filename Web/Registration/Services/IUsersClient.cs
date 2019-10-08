using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Registration.Models;
using Registration.ViewModels;
using Registration.Views.Registration;

namespace Registration.Services
{
    public interface IUsersClient
    {
        Task<User> CreateUserAsync(RegistrationFormDto registrationFormDto);

        Task ConfirmEmailAsync(EmailConfirmationDto emailConfirmationDto);
        Task ResetPasswordAsync(PasswordResetDto passwordResetDto);
        Task CompletePasswordResetAsync(PasswordResetCompletionPostDto dto);
    }
}
