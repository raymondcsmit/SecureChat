using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Chat.FunctionalTests.Scenarios
{
    public abstract class ChatScenarioBase
    {
        public TestServer CreateServer()
        {
            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Helpers.TestRoot)
                .ConfigureAppConfiguration((ctx, cb) =>
                {
                    cb.SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                }).UseStartup<ChatTestStartup>();

            var testServer = new TestServer(hostBuilder);

            return testServer;
        }

        private const string UsersBaseUrl = "http://localhost/api/users";

        public static class Get
        {
            public static string Users = UsersBaseUrl;

            public static string UserById(string id) 
                => $"{UsersBaseUrl}/{id}";
        }

        public static class Patch
        {
            public static string Users = UsersBaseUrl;

            public static string UpdateUserById(string id)
                => $"{UsersBaseUrl}/{id}";
        }
    }
}
