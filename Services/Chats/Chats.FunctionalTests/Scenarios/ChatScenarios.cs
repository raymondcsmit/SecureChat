using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chats.Api.Application.Queries;
using Chats.Api.Application.Specifications;
using Chats.Api.Dtos;
using Chats.Domain.AggregateModel;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Chats.FunctionalTests.Scenarios
{
    public class ChatScenarios :
        ChatsScenarioBase,
        IClassFixture<TestServerFixture>,
        IClassFixture<TestEventBusFixture>,
        IDisposable
    {
        public TestServerFixture TestServerFixture { get; }
        public TestEventBusFixture TestEventBusFixture { get; }

        public ChatScenarios(TestServerFixture testServerFixture, TestEventBusFixture testEventBusFixture)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            TestServerFixture = testServerFixture;
            TestEventBusFixture = testEventBusFixture;
            TestServerFixture.TestServer ??= CreateServer();
        }

        [Fact]
        public async Task CreateChat_ShouldCreateChat_WithNewName()
        {
            var client = CreateClient();

            var chat = new
            {
                OwnerId = "system",
                Name = "TestChat",
                Capacity = 20
            };

            var response = await client.PostAsJsonAsync(Post.Chats, chat);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            using var scope = TestServerFixture.TestServer.Host.Services.CreateScope();
            var queries = scope.ServiceProvider.GetRequiredService<IChatQueries>();
            var (result, total) =
                await queries.GetChatsForOwnerOrMemberAsync(new ChatSpecification(new QueryDto()), "system");
            Assert.Equal(1, total);
            Assert.True(result.First().Owner.Id == "system");
        }

        [Fact]
        public async Task CreateChat_ShouldFail_WithExistingName()
        {
            var client = CreateClient();

            var chat = new
            {
                OwnerId = "system",
                Name = "TestChat",
                Capacity = 20
            };

            await client.PostAsJsonAsync(Post.Chats, chat);
            var response = await client.PostAsJsonAsync(Post.Chats, chat);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            using var scope = TestServerFixture.TestServer.Host.Services.CreateScope();
            var queries = scope.ServiceProvider.GetRequiredService<IChatQueries>();
            var (result, total) =
                await queries.GetChatsForOwnerOrMemberAsync(new ChatSpecification(new QueryDto()), "system");
            Assert.Equal(1, total);
        }

        private HttpClient CreateClient()
        {
            var client = TestServerFixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Add("BypassAuthentication", "secret");
            return client;
        }

        private async Task CreateTestChatAsync(Chat chat)
        {
            using var scope = TestServerFixture.TestServer.Host.Services.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IChatRepository>();
            userRepository.Create(chat);
            await userRepository.UnitOfWork.SaveChangesAsync();
        }

        public void Dispose()
        {
            TestEventBusFixture.EventBus.ClearCallbacks();
            TestServerFixture.ClearChats().Wait();
        }
    }
}
