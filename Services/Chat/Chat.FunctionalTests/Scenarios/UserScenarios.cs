using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chat.API.Application.IntegrationEvents.Events;
using Chat.Domain.AggregateModel.UserAggregate;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Chat.FunctionalTests.Scenarios
{
    public class UserScenarios : 
        ChatScenarioBase, 
        IClassFixture<TestServerFixture>,
        IClassFixture<TestEventBusFixture>,
        IDisposable
    {
        public TestServerFixture TestServerFixture { get; }
        public TestEventBusFixture TestEventBusFixture { get; }

        public UserScenarios(TestServerFixture testServerFixture, TestEventBusFixture testEventBusFixture)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            TestServerFixture = testServerFixture;
            TestEventBusFixture = testEventBusFixture;
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
        }

        [Fact]
        public async Task UpdateUserById_ShouldCreateProfile_WhenProfileDoesNotExist()
        {
            var client = CreateClient();

            var user = new User("1", "alice", "alice@alice.com");
            await CreateTestUserAsync(user);

            var profile = new
            {
                age = 20,
                sex = "M",
                location = "PA"
            };
            var patch = new dynamic[]
            {
                new {op = "add", path = "/profile", value = profile},
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(user.Id), content);
            var responseStr = await patchResponse.Content.ReadAsStringAsync();

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var updatedUser = await userRepository.GetAsync(user.Id);
                Assert.True(updatedUser.HasProfile);
                Assert.True(updatedUser.Profile.Age == profile.age && 
                            updatedUser.Profile.Sex == profile.sex &&
                            updatedUser.Profile.Location == profile.location);
            }
        }

        [Fact]
        public async Task UpdateUserById_ShouldPartiallyUpdateProfile_WhenProfileExists()
        {
            var client = CreateClient();

            var user = new User("1", "alice", "alice@alice.com")
            {
                Profile = new Profile(20, "M", "PA")
            };
            await CreateTestUserAsync(user);

            var patch = new dynamic[]
            {
                new {op = "replace", path = "/profile/age", value = 21},
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(user.Id), content);
            var responseStr = await patchResponse.Content.ReadAsStringAsync();

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var updatedUser = await userRepository.GetAsync(user.Id);
                Assert.True(updatedUser.HasProfile);
                Assert.True(updatedUser.Profile.Age == 21);
            }
        }

        [Theory]
        [InlineData("replace", "/username", "Bar")]
        [InlineData("replace", "/email", "bar@bar.com")]
        public async Task UpdateUserById_ShouldDispatchIntegrationEvent_OnEmailUserNameUpdate(string op, string path, string value)
        {
            var client = CreateClient();
            var user = new User("1", "Foo", "foo@foo.com");
            await CreateTestUserAsync(user);

            string eventName = null;
            TestEventBusFixture.EventBus.Subscribe(nameof(UserAccountUpdatedIntegrationEvent), (e, _) => eventName = e);
            await Task.Delay(1000);

            var patch = new dynamic[]
            {
                new {op, path, value}
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(user.Id), content);
            await Task.Delay(1000);

            Assert.True(eventName == nameof(UserAccountUpdatedIntegrationEvent));
        }

        [Fact]
        public async Task AccountCreatedIntegrationEventHandler_ShouldCreateUserWithoutProfile_OnUserAccountCreatedIntegrationEvent()
        {
            var user = new
            {
                Id = "1",
                UserName = "Foo",
                Email = "foo@foo.com"
            };
            TestEventBusFixture.EventBus.Publish(
                new UserAccountCreatedIntegrationEvent(user.Id, user.UserName, user.Email));
            await Task.Delay(1000);

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var createdUser = await userRepository.GetAsync(user.Id);
                Assert.NotNull(createdUser);
                Assert.True(!createdUser.HasProfile);
                Assert.True(createdUser.UserName == user.UserName);
                Assert.True(createdUser.Email == user.Email);
            }
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

        public void Dispose()
        {
            TestEventBusFixture.EventBus.ClearCallbacks();
            TestServerFixture.ClearUsers().Wait();
        }
    }
}
