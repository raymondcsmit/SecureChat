using Account.API.Infrastructure;
using Helpers.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.FunctionalTests
{
    public abstract class AccountScenarioBase
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
                }).UseStartup<AccountTestStartup>();

            var testServer = new TestServer(hostBuilder);

            testServer.Host
                .MigrateDbContext<AccountDbContext>(services =>
                {
                    var usersDbContextSeed = services.GetRequiredService<AccountDbContextSeed>();
                    usersDbContextSeed.SeedAsync().Wait();
                });

            return testServer;
        }

        private static readonly string BaseUrl = "http://localhost/api/users";

        public static class Get
        {
            public static string Users = BaseUrl;

            public static string UserById(string id) 
                => $"{BaseUrl}/{id}";
        }

        public static class Post
        {
            public static readonly string CreateUser = BaseUrl;

            public static string ResetPasswordByUserName(string userName) 
                => $"{BaseUrl}/{userName}/reset-password";

            public static string CompletePasswordResetById(string id) 
                => $"{BaseUrl}/{id}/complete-password-reset";

            public static string ConfirmEmailById(string id)
                => $"{BaseUrl}/{id}/confirm-email";
        }

        public static class Patch
        {
            public static string Users = BaseUrl;

            public static string UpdateUserById(string id)
                => $"{BaseUrl}/{id}";
        }
    }
}
