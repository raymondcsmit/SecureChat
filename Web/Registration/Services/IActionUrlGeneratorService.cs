namespace Registration.Services
{
    public interface IActionUrlGeneratorService
    {
        string GetUrl(string actionName);
        string GetUrl(string actionName, object queryParams);
    }
}