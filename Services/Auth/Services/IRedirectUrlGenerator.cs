namespace Auth.Services
{
    public interface IRedirectUrlGenerator
    {
        string GenerateUrl(string baseUrl);
    }
}
