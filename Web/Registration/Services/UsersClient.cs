using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Registration.Controllers;
using Registration.Exceptions;
using Registration.Infrastructure;
using Registration.Models;
using Registration.ViewModels;
using Registration.Views.Registration;

namespace Registration.Services
{
    public class UsersClient : IUsersClient
    {
        private readonly HttpClient _httpClient;
        private readonly IActionUrlGeneratorService _actionUrlGeneratorService;

        public UsersClient(
            HttpClient httpClient,
            IActionUrlGeneratorService actionUrlGeneratorService)
        {
            _httpClient = httpClient;
            _actionUrlGeneratorService = actionUrlGeneratorService;
        }

        public async Task<User> CreateUserAsync(RegistrationFormDto registrationFormDto)
        {
            using (var response = await _httpClient.PostAsJsonAsync("api/users", registrationFormDto))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessResponse<User>(response);
                }

                throw await ApiException.FromHttpResponse(response);
            }
        }

        public async Task ConfirmEmailAsync(EmailConfirmationDto emailConfirmationDto)
        {
            using (var response = await _httpClient.PostAsJsonAsync($"api/users/{emailConfirmationDto.Id}/confirm-email", emailConfirmationDto))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                throw await ApiException.FromHttpResponse(response);
            }
        }

        public async Task ResetPasswordAsync(PasswordResetDto passwordResetDto)
        {
            var body = new
            {
                CompletionUrl = _actionUrlGeneratorService.GetUrl(nameof(PasswordResetController.CompletePasswordResetGet), new
                {
                    passwordResetDto.LoginUrl
                })
            };
            using (var response = await _httpClient.PostAsJsonAsync($"api/users/{passwordResetDto.UserName}/reset-password", body))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                throw await ApiException.FromHttpResponse(response);
            }
        }

        public async Task CompletePasswordResetAsync(PasswordResetCompletionPostDto passwordResetDto)
        {
            var body = new
            {
                Token = passwordResetDto.Token,
                UserName = passwordResetDto.UserName,
                NewPassword = passwordResetDto.Password
            };
            using (var response = await _httpClient.PostAsJsonAsync($"api/users/{passwordResetDto.UserName}/complete-password-reset", body))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                throw await ApiException.FromHttpResponse(response);
            }
        }

        private async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
