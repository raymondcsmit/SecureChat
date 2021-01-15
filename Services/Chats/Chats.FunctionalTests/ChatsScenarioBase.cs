using Chats.Infrastructure;
using Helpers.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chats.FunctionalTests
{
    public abstract class ChatsScenarioBase
    {
        public TestServer CreateServer()
        {
            var hostBuilder = WebHost.CreateDefaultBuilder()
                .UseContentRoot(Helpers.TestRoot)
                .ConfigureAppConfiguration((ctx, cb) =>
                {
                    cb.SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                }).UseStartup<ChatsTestStartup>();

            var testServer = new TestServer(hostBuilder);
            testServer.Host.MigrateDbContext<ChatsContext>();

            return testServer;
        }

        private const string ChatsBaseUrl = "http://localhost/api/chats";

        public static class Get
        {
            public static string Chats
                => ChatsBaseUrl;

            public static string ChatById(string id)
                => $"{ChatsBaseUrl}/{id}";

        }

        public static class Post
        {
            public static string Chats = ChatsBaseUrl;
        }
    }
}
