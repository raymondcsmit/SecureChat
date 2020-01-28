using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Account.API.Application.IntegrationEvents.Events;
using Account.API.Dtos;
using Account.API.Models;
using Account.FunctionalTests.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using SecureChat.Common.Events.EventBus.Abstractions;
using Xunit;

namespace Account.FunctionalTests
{
    public class AccountScenarios : 
        AccountScenarioBase, 
        IClassFixture<TestServerFixture>, 
        IClassFixture<TestEventBusFixture>, 
        IDisposable
    {
        public TestServerFixture TestServerFixture { get; }
        public TestEventBusFixture TestEventBusFixture { get; }

        public AccountScenarios(TestServerFixture testServerFixture, TestEventBusFixture testEventBusFixture)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            TestServerFixture = testServerFixture;
            TestEventBusFixture = testEventBusFixture;
            if (TestServerFixture.TestServer == null)
            {
                TestServerFixture.TestServer = CreateServer();
            }
        }

        [Theory]
        [InlineData(null, "foo@bar", "password")]
        [InlineData("foo", null, "password")]
        [InlineData("foo", "foo@bar", null)]
        public async Task CreateUser_ShouldReturnBadRequest_OnInvalidData(string username, string email, string password)
        {
            var client = CreateClient();
            var user = new
            {
                UserName = username,
                Email = email,
                Password = password
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_OnInvalidEmail()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "gibberish",
                Password = "P@ssword1"
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_OnDuplicateEmail()
        {
            var client = CreateClient();
            var user1 = new
            {
                UserName = "Foo",
                Email = "foo@foo.com",
                Password = "P@ssword1"
            };
            var user2 = new
            {
                UserName = "Bar",
                Email = "foo@foo.com",
                Password = "P@ssword1"
            };

            await client.PostAsJsonAsync(Post.CreateUser, user1);
            var response = await client.PostAsJsonAsync(Post.CreateUser, user2);
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreated_OnValidData()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            Assert.True(response.StatusCode == HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUserInBody_OnValidData()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            var responseBody = await response.DeserializeAsync<UserDto>();
            Assert.Equal(user.UserName, responseBody.UserName);
        }

        [Fact]
        public async Task CreateUser_ShouldCreateUser_OnValidData()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            var responseBody = await response.DeserializeAsync<UserDto>();
            var userManager = TestServerFixture.TestServer.Host.Services.GetRequiredService<UserManager<User>>();
            var createdUser = await userManager.FindByIdAsync(responseBody.Id);
            Assert.NotNull(createdUser);
        }

        [Fact]
        public async Task CreateUser_ShouldSetValidTimestamp_OnValidData()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            var responseBody = await response.DeserializeAsync<UserDto>();
            var creationTimestamp = DateTimeOffset.Parse(responseBody.CreatedAt);
            var rangeBegin = DateTimeOffset.Now.AddSeconds(-2);
            var rangeEnd = DateTimeOffset.Now.AddSeconds(2);
            var x = rangeBegin < creationTimestamp;
            var y = rangeEnd > creationTimestamp;
            Assert.True(rangeBegin < creationTimestamp && rangeEnd > creationTimestamp);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_OnValidId()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var postResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var postResponseBody = await postResponse.DeserializeAsync<UserDto>();
            var getResponse = await client.GetAsync(Get.UserById(postResponseBody.Id));
            Assert.True(getResponse.IsSuccessStatusCode);
            var getResponseBody = await getResponse.DeserializeAsync<UserDto>();
            Assert.Equal(postResponseBody.Id, getResponseBody.Id);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_OnInvalidId()
        {
            var client = CreateClient();
            var getResponse = await client.GetAsync(Get.UserById("gibberish"));
            Assert.True(getResponse.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateUser_ShouldSendEmailToUser_OnValidData()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var response = await client.PostAsJsonAsync(Post.CreateUser, user);
            AccountTestStartup.EmailSenderMock.Verify(mock =>
                mock.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldConfirmUserEmail_OnValidToken()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var responseBody = await createUserResponse.DeserializeAsync<UserDto>();
            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var createdUser = await userManager.FindByNameAsync(user.UserName);
                var token = await userManager.GenerateEmailConfirmationTokenAsync(createdUser);
                await client.PostAsJsonAsync(Post.ConfirmEmailById(createdUser.Id), new {Token = token});
            }

            // DbContext has request scope. Need to create new scope to observe change to EmailConfirmed flag.
            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var createdUser = await userManager.FindByNameAsync(user.UserName);
                Assert.True(await userManager.IsEmailConfirmedAsync(createdUser));
            }
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnBadRequest_OnInvalidToken()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            var confirmEmailResponse = await client.PostAsJsonAsync(Post.ConfirmEmailById(createUserResponseBody.Id), new { Token = "gibberish" });
            Assert.True(confirmEmailResponse.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldResendConfirmationEmail_OnMissingToken()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            AccountTestStartup.EmailSenderMock.Reset();
            await client.PostAsJsonAsync(Post.ConfirmEmailById(createUserResponseBody.Id), new { });
            AccountTestStartup.EmailSenderMock.Verify(mock =>
                mock.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnNotFound_OnInvalidUser()
        {
            var client = CreateClient();
            var confirmEmailResponse = await client.PostAsJsonAsync(Post.ConfirmEmailById("gibberish"), new { Token = "gibberish" });
            Assert.True(confirmEmailResponse.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ResetPassword_ShouldSendEmailToUser_OnValidUsername()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            AccountTestStartup.EmailSenderMock.Reset();
            await client.PostAsJsonAsync(Post.ResetPasswordByUserName(user.UserName), new { CompletionUrl = "foo.bar"});
            AccountTestStartup.EmailSenderMock.Verify(mock =>
                mock.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ShouldNotSendEmail_OnInvalidUsername()
        {
            var client = CreateClient();
            await client.PostAsJsonAsync(Post.ResetPasswordByUserName("gibberish"), new { CompletionUrl = "foo.bar" });
            AccountTestStartup.EmailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnOk_OnInvalidUsername()
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(Post.ResetPasswordByUserName("gibberish"), new { CompletionUrl = "foo.bar" });
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task CompletePasswordReset_ShouldResetPassword_OnValidIdAndToken()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            const string newPassword = "P@ssword2";
            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var createdUser = await userManager.FindByIdAsync(createUserResponseBody.Id);
                var token = await userManager.GeneratePasswordResetTokenAsync(createdUser);
                await client.PostAsJsonAsync(Post.CompletePasswordResetById(createdUser.Id), new { Token = token, NewPassword = newPassword });
            }

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var createdUser = await userManager.FindByIdAsync(createUserResponseBody.Id);
                Assert.True(await userManager.CheckPasswordAsync(createdUser, newPassword));
            }
        }

        [Fact]
        public async Task CompletePasswordReset_ShouldReturn404_OnInvalidId()
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(Post.CompletePasswordResetById("gibberish"), new { Token = "gibberish", NewPassword = "P@ssword2" });
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CompletePasswordReset_ShouldReturnBadRequest_OnInvalidToken()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            const string newPassword = "P@ssword2";
            var completePasswordResetResponse = await client.PostAsJsonAsync(Post.CompletePasswordResetById(createUserResponseBody.Id), new { Token = "gibberish", NewPassword = newPassword });
            Assert.True(completePasswordResetResponse.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CompletePasswordReset_ShouldNotChangePassword_OnInvalidToken()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            const string newPassword = "P@ssword2";
            await client.PostAsJsonAsync(Post.CompletePasswordResetById(createUserResponseBody.Id), new { Token = "gibberish", NewPassword = newPassword });

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var createdUser = await userManager.FindByIdAsync(createUserResponseBody.Id);
                Assert.True(await userManager.CheckPasswordAsync(createdUser, user.Password));
            }
        }

        [Fact]
        public async Task ServiceEndpoints_ShouldNotReturnUnauthorized_OnValidBypassSecret()
        {
            var client = TestServerFixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Add("BypassAuthentication", "secret");
            var getResponse = await client.GetAsync(Get.UserById("gibberish"));
            Assert.False(getResponse.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ServiceEndpoints_ShouldReturnUnauthorized_OnInvalidBypassSecret()
        {
            var client = TestServerFixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Add("BypassAuthentication", "gibberish12345");
            var getResponse = await client.GetAsync(Get.UserById("gibberish"));
            Assert.True(getResponse.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(null, "bar@bar.com")]
        [InlineData("Bar", null)]
        [InlineData("Bar", "foo@bar.com")]
        public async Task AccountUpdatedIntegrationEventHandler_ShouldChangeUserNameEmail_OnUserAccountUpdatedIntegrationEvent(string userName, string email)
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@bar.com",
                Password = "P@ssword1"
            };

            var userNameAfterUpdate = userName ?? user.UserName;
            var emailAfterUpdate = email ?? user.Email;

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            TestEventBusFixture.EventBus.Publish(
                new UserAccountUpdatedIntegrationEvent(createUserResponseBody.Id, userName, email));

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                await Task.Delay(1000);
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var createdUser = await userManager.FindByIdAsync(createUserResponseBody.Id);
                Assert.True(createdUser.Email == emailAfterUpdate);
                Assert.True(createdUser.UserName == userNameAfterUpdate);
            }
        }

        public void Dispose()
        {
            TestEventBusFixture.EventBus.ClearCallbacks();
            TestServerFixture.ClearUsers().Wait();
            AccountTestStartup.EmailSenderMock.Reset();
        }

        private HttpClient CreateClient()
        {
            var client = TestServerFixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Add("BypassAuthentication", "secret");
            return client;
        }
    }
}
