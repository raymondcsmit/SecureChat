using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Chat.FunctionalTests.Scenarios
{
    public class UserScenarios : ChatScenarioBase, IClassFixture<TestServerFixture>
    {
        public TestServerFixture TestServerFixture { get; }

        public UserScenarios(TestServerFixture testServerFixture)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            TestServerFixture = testServerFixture;
            if (TestServerFixture.TestServer == null)
            {
                TestServerFixture.TestServer = CreateServer();
            }
        }

        [Fact]
        public async Task UpdateUserById_ShouldPerformPartialUpdate_OnValidData()
        {
            var client = CreateClient();

            var user = new User("1", "alice", "alice@alice.com");
            await CreateTestUserAsync(user);

            var patch = new dynamic[]
            {
                new {op = "replace", path = "/email", value = "bar@bar.com"},
                new {op = "replace", path = "/username", value = "bar"}
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(user.Id), content);
            var responseStr = await patchResponse.Content.ReadAsStringAsync();

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var updatedUser = await userRepository.GetAsync(user.Id);
                Assert.True(updatedUser.Email == "bar@bar.com");
                Assert.True(updatedUser.UserName == "bar");
            }
        }

        [Theory]
        [InlineData("replace", "/username", "Bar")]
        [InlineData("replace", "/email", "bar@bar.com")]
        public async Task UpdateUserById_ShouldReturnBadRequest_OnInvalidData(string op, string path, string value)
        {
            var client = CreateClient();
            var user1 = new User("1", "Foo", "foo@foo.com");
            var user2 = new User("2", "Bar", "bar@bar.com");
            foreach (var user in new[] {user1, user2})
            {
                await CreateTestUserAsync(user);
            }

            var patch = new dynamic[]
            {
                new {op, path, value}
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(user1.Id), content);
            Assert.True(patchResponse.StatusCode == HttpStatusCode.BadRequest);

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var updatedUser = await userRepository.GetAsync(user1.Id);
                Assert.True(updatedUser.Email == "foo@foo.com");
                Assert.True(updatedUser.UserName == "Foo");
            }

            await TestServerFixture.ClearUsers();
        }

        private HttpClient CreateClient()
        {
            var client = TestServerFixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Add("BypassAuthentication", "secret");
            return client;
        }

        private async Task CreateTestUserAsync(User user)
        {
            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                userRepository.Add(user);
                await userRepository.UnitOfWork.SaveChangesAsync();
            }
        }
    }
}
