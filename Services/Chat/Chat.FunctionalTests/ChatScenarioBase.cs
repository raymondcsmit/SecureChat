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
        private const string FriendshipRequestsBaseUrl = "http://localhost/api/friendship-requests";
        private const string FriendshipsBaseUrl = "http://localhost/api/friendships";

        public static class Get
        {
            public static string FriendshipRequests()
                => FriendshipRequestsBaseUrl;

            public static string FriendshipRequestById(string id)
                => $"{FriendshipRequestsBaseUrl}/{id}";

            public static string FriendshipRequestsByRequesteeId(string requesteeId)
                => $"{UsersBaseUrl}/{requesteeId}/friendship-requests";

            public static string FriendshipsByUserId(string userId)
                => $"{UserById(userId)}/friendships";

            public static string Users()
                => UsersBaseUrl;

            public static string UserById(string id) 
                => $"{UsersBaseUrl}/{id}";
            public static string Me()
                => $"{UsersBaseUrl}/me";
        }

        public static class Post
        {
            public static string FriendshipRequests = FriendshipRequestsBaseUrl;
        }

        public static class Patch
        {
            public static string Users = UsersBaseUrl;

            public static string UserById(string id)
                => $"{UsersBaseUrl}/{id}";

            public static string UpdateFriendshipRequestStatusById(string id)
                => $"{FriendshipRequestsBaseUrl}/{id}";
        }
    }
}
