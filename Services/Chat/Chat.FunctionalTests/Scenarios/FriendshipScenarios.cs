using Chat.API.Dtos;
using Chat.API.Models;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Chat.FunctionalTests.Scenarios
{
    public class FriendshipScenarios :
        ChatScenarioBase,
        IClassFixture<TestServerFixture>,
        IClassFixture<TestEventBusFixture>,
        IDisposable
    {
        public TestServerFixture TestServerFixture { get; }
        public TestEventBusFixture TestEventBusFixture { get; }

        public FriendshipScenarios(TestServerFixture testServerFixture, TestEventBusFixture testEventBusFixture)
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
        public async Task CreateFriendshipRequest_ShouldCreateFriendshipRequest_ForExistingUsers()
        {
            var client = CreateClient();

            var alice = new User("1", "alice", "alice@alice.com");
            var bob = new User("2", "bob", "bob@bob.com");
            await CreateTestUserAsync(alice);
            await CreateTestUserAsync(bob);

            var friendshipRequest = new
            {
                RequesterId = alice.Id,
                RequesteeId = bob.Id
            };

            var response = await client.PostAsJsonAsync(Post.FriendshipRequests, friendshipRequest);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created);

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IFriendshipRequestRepository>();
                var result = await repo.GetByUserIdAsync(bob.Id);
                Assert.NotEmpty(result);
                Assert.True(result.First().RequesteeId == bob.Id);
            }
        }

        [Fact]
        public async Task CreateFriendshipRequest_ShouldReturnBadRequest_ForNonexistingUsers()
        {
            var client = CreateClient();

            var alice = new User("1", "alice", "alice@alice.com");
            var bob = new User("2", "bob", "bob@bob.com");
            await CreateTestUserAsync(alice);
            await CreateTestUserAsync(bob);

            var friendshipRequest = new
            {
                RequesterId = alice.Id,
                RequesteeId = new Guid().ToString()
            };

            var response = await client.PostAsJsonAsync(Post.FriendshipRequests, friendshipRequest);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IFriendshipRequestRepository>();
                var result = await repo.GetByUserIdAsync(bob.Id);
                Assert.Empty(result);
                result = await repo.GetByUserIdAsync(alice.Id);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetFriendshipRequestById_ShouldReturnFriendshipRequest_ForExistingFriendshipRequest()
        {
            var client = CreateClient();

            var alice = new User("1", "alice", "alice@alice.com");
            var bob = new User("2", "bob", "bob@bob.com");
            await CreateTestUserAsync(alice);
            await CreateTestUserAsync(bob);

            var friendshipRequestId = await CreateTestFriendshipRequestAsync(alice.Id, bob.Id);

            var response = await client.GetAsync(Get.FriendshipRequestById(friendshipRequestId));
            Assert.True(response.IsSuccessStatusCode);
            var responseStr = await response.Content.ReadAsStringAsync();
            var friendshipRequestResponse = JsonConvert.DeserializeObject<FriendshipRequestDto>(responseStr);
            Assert.True(friendshipRequestResponse.Id == friendshipRequestId);
            Assert.True(friendshipRequestResponse.Requester.Id == alice.Id);
            Assert.True(friendshipRequestResponse.Requestee.Id == bob.Id);
        }

        [Fact]
        public async Task GetFriendshipRequestById_ShouldReturnNotFound_ForNonexistingFriendshipRequest()
        {
            var client = CreateClient();

            var response = await client.GetAsync(Get.FriendshipRequestById("gibberish"));
            Assert.False(response.IsSuccessStatusCode);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetFriendshipRequests_ShouldReturnListOfFriendshipRequests_OnMatchingCriteria()
        {
            var client = CreateClient();

            var alice = new User("1", "alice", "alice@alice.com");
            var bob = new User("2", "bob", "bob@bob.com");
            await CreateTestUserAsync(alice);
            await CreateTestUserAsync(bob);

            var friendshipRequestId = await CreateTestFriendshipRequestAsync(alice.Id, bob.Id);

            var query = new
            {
                criteria = new { id = friendshipRequestId },
                pagination = new
                {
                    limit = 10,
                    offset = 0
                }
            };

            var uri = UriHelpers.BuildUri(Get.FriendshipRequests(), new { query = JsonConvert.SerializeObject(query) });
            var response = await client.GetAsync(uri);
            var responseStr = await response.Content.ReadAsStringAsync();
            var responseArr = JsonConvert.DeserializeObject<ArrayResponse<FriendshipRequestDto>>(responseStr);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.True(responseArr.Items.Count() == 1);
            var friendshipRequest = responseArr.Items.First();
            Assert.True(friendshipRequest.Id == friendshipRequestId);
            Assert.True(friendshipRequest.Requester.Id == alice.Id);
            Assert.True(friendshipRequest.Requestee.Id == bob.Id);
            Assert.True(responseArr.Total == 1);
        }

        [Fact]
        public async Task GetFriendshipRequests_ShouldReturnEmptyList_OnNoMatchingCriteria()
        {
            var client = CreateClient();

            var query = new
            {
                criteria = new { id = "gibberish" },
                pagination = new
                {
                    limit = 10,
                    offset = 0
                }
            };

            var uri = UriHelpers.BuildUri(Get.FriendshipRequests(), new { query = JsonConvert.SerializeObject(query) });
            var response = await client.GetAsync(uri);
            var responseStr = await response.Content.ReadAsStringAsync();
            var responseArr = JsonConvert.DeserializeObject<ArrayResponse<FriendshipRequestDto>>(responseStr);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.True(!responseArr.Items.Any());
            Assert.True(responseArr.Total == 0);
        }

        [Fact]
        public async Task GetFriendshipRequestsByRequesteeId_ShouldReturnListOfFriendshipRequests_OnExistingFriendshipRequestsAndExistingUser()
        {
            var client = CreateClient();

            var alice = new User("1", "alice", "alice@alice.com");
            var bob = new User("2", "bob", "bob@bob.com");
            await CreateTestUserAsync(alice);
            await CreateTestUserAsync(bob);

            var friendshipRequestId = await CreateTestFriendshipRequestAsync(alice.Id, bob.Id);

            var query = new
            {
                pagination = new
                {
                    limit = 10,
                    offset = 0
                }
            };

            var uri = UriHelpers.BuildUri(Get.FriendshipRequestsByRequesteeId(bob.Id), new { query = JsonConvert.SerializeObject(query) });
            var response = await client.GetAsync(uri);
            var responseStr = await response.Content.ReadAsStringAsync();
            var responseArr = JsonConvert.DeserializeObject<ArrayResponse<FriendshipRequestDto>>(responseStr);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.True(responseArr.Items.Count() == 1);
            var friendshipRequest = responseArr.Items.First();
            Assert.True(friendshipRequest.Id == friendshipRequestId);
            Assert.True(friendshipRequest.Requester.Id == alice.Id);
            Assert.True(friendshipRequest.Requestee.Id == bob.Id);
            Assert.True(responseArr.Total == 1);
        }

        [Fact]
        public async Task GetFriendshipRequestsByRequesteeId_ShouldReturnEmptyList_OnNoExistingFriendshipRequestsAndExistingUser()
        {
            var client = CreateClient();

            var alice = new User("1", "alice", "alice@alice.com");
            await CreateTestUserAsync(alice);

            var query = new
            {
                pagination = new
                {
                    limit = 10,
                    offset = 0
                }
            };

            var uri = UriHelpers.BuildUri(Get.FriendshipRequestsByRequesteeId(alice.Id), new { query = JsonConvert.SerializeObject(query) });
            var response = await client.GetAsync(uri);
            var responseStr = await response.Content.ReadAsStringAsync();
            var responseArr = JsonConvert.DeserializeObject<ArrayResponse<FriendshipRequestDto>>(responseStr);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.True(!responseArr.Items.Any());
            Assert.True(responseArr.Total == 0);
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
                userRepository.Create(user);
                await userRepository.UnitOfWork.SaveChangesAsync();
            }
        }

        private async Task<string> CreateTestFriendshipRequestAsync(string requesterId, string requesteeId)
        {
            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IFriendshipRequestRepository>();
                var friendshipRequest = new FriendshipRequest(requesterId, requesteeId);
                repo.Create(friendshipRequest);
                await repo.UnitOfWork.SaveChangesAsync();
                return friendshipRequest.Id;
            }
        }

        public void Dispose()
        {
            TestEventBusFixture.EventBus.ClearCallbacks();
            TestServerFixture.ClearUsers().Wait();
        }
    }
}
