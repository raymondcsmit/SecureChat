namespace Users.API.Services.Email
{
    public interface IEmailGenerator
    {
        (string, string) GenerateEmailConfirmationEmail(string userName, string token);

        (string, string) GeneratePasswordResetEmail(string userName, string token, string completionUrl);
    }
}
