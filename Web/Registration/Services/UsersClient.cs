using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Registration.Controllers;
using Registration.Exceptions;
using Registration.Models;
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

        public async Task ResetPasswordAsync(string userName, string loginUrl)
        {
            var body = new
            {
                CompletionUrl = _actionUrlGeneratorService.GetUrl(nameof(PasswordResetController.CompletePasswordResetGet), new
                {
                    LoginUrl = loginUrl ?? string.Empty
                })
            };
            using (var response = await _httpClient.PostAsJsonAsync($"api/users/{userName}/reset-password", body))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                throw await ApiException.FromHttpResponse(response);
            }
        }

        public async Task CompletePasswordResetAsync(string userName, string token, string password)
        {
            var body = new
            {
                Token = token,
                UserName = userName,
                NewPassword = password
            };
            using (var response = await _httpClient.PostAsJsonAsync($"api/users/{userName}/complete-password-reset", body))
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
