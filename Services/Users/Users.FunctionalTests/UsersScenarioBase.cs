using Helpers.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.API.Infrastructure;

namespace Users.FunctionalTests
{
    public abstract class UsersScenarioBase
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
                }).UseStartup<UsersTestStartup>();

            var testServer = new TestServer(hostBuilder);

            testServer.Host
                .MigrateDbContext<UsersDbContext>(services =>
                {
                    var usersDbContextSeed = services.GetRequiredService<UsersDbContextSeed>();
                    usersDbContextSeed.SeedAsync().Wait();
                });

            return testServer;
        }

        public void DeleteDatabase(TestServer testServer)
        {
            using (var scope = testServer.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                dbContext.Database.EnsureDeleted();
            }
        }

        private static readonly string BaseUrl = "api/users";

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
    }
}
