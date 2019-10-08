using System.Collections.Specialized;
using Helpers;
using Microsoft.Extensions.Configuration;

namespace Users.API.Services.Email
{
    public class DefaultEmailGenerator : IEmailGenerator
    {
        private readonly IConfiguration _configuration;

        public DefaultEmailGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string, string) GenerateEmailVerificationEmail(string userName, string token)
        {
            var emailVerificationUrl = _configuration["EmailConfirmationUrl"];
            var url = UriHelpers.BuildUri(emailVerificationUrl, new NameValueCollection()
            {
                {"UserName", userName},
                {"Token", token}
            }).ToString();
            return ("Please confirm your email",
                $@"Thank you for registering with SecureChat. Please click <a href=""{url}"">here</a> to verify your email address.");
        }

        public (string, string) GeneratePasswordResetEmail(string userName, string token, string completionUrl)
        {
            var url = UriHelpers.BuildUri(completionUrl, new NameValueCollection()
            {
                {"UserName", userName},
                {"Token", token}
            }).ToString();
            return ("Your password reset request",
                $@"Click <a href=""{url}"">here</a> to reset your password.");
        }
    }
}
