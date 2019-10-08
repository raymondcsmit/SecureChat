namespace Registration.Services
{
    public interface IUrlGeneratorService
    {
        string GetUrl(string actionName);
        string GetUrl(string actionName, object queryParams);
    }
}