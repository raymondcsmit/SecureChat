using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Users.API.Dtos;
using Users.API.Models;
using Users.FunctionalTests.Extensions;
using Xunit;

namespace Users.FunctionalTests
{
    public class UsersScenarios : UsersScenarioBase, IClassFixture<TestServerFixture>, IDisposable
    {
        public TestServerFixture TestServerFixture { get; }

        public UsersScenarios(TestServerFixture testServerFixture)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            TestServerFixture = testServerFixture;
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
            UsersTestStartup.EmailSenderMock.Verify(mock =>
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
            UsersTestStartup.EmailSenderMock.Reset();
            await client.PostAsJsonAsync(Post.ConfirmEmailById(createUserResponseBody.Id), new { });
            UsersTestStartup.EmailSenderMock.Verify(mock =>
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
            UsersTestStartup.EmailSenderMock.Reset();
            await client.PostAsJsonAsync(Post.ResetPasswordByUserName(user.UserName), new { CompletionUrl = "foo.bar"});
            UsersTestStartup.EmailSenderMock.Verify(mock =>
                mock.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ShouldNotSendEmail_OnInvalidUsername()
        {
            var client = CreateClient();
            await client.PostAsJsonAsync(Post.ResetPasswordByUserName("gibberish"), new { CompletionUrl = "foo.bar" });
            UsersTestStartup.EmailSenderMock.Verify(mock =>
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

        [Fact]
        public async Task UpdateUserById_ShouldPerformPartialUpdate_OnValidData()
        {
            var client = CreateClient();
            var user = new
            {
                UserName = "Foo",
                Email = "foo@foo.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            var patch = new dynamic[]
            {
                new {op = "replace", path = "/email", value = "foo@foo.com"},
                new {op = "replace", path = "/username", value = "bar"}
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(createUserResponseBody.Id), content);
            var responseStr = await patchResponse.Content.ReadAsStringAsync();

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var updatedUser = await userManager.FindByIdAsync(createUserResponseBody.Id);
                Assert.True(updatedUser.Email == "foo@foo.com");
                Assert.True(updatedUser.UserName == "bar");
            }
        }

        [Theory]
        [InlineData("replace", "/username", "Bar")]
        [InlineData("replace", "/email", "bar@bar.com")]
        public async Task UpdateUserById_ShouldReturnBadRequest_OnInvalidData(string op, string path, string value)
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
                Email = "bar@bar.com",
                Password = "P@ssword1"
            };

            var createUserResponse = await client.PostAsJsonAsync(Post.CreateUser, user1);
            var createUserResponseBody = await createUserResponse.DeserializeAsync<UserDto>();
            await client.PostAsJsonAsync(Post.CreateUser, user2);
            var patch = new dynamic[]
            {
                new {op, path, value}
            };
            var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
            var patchResponse = await client.PatchAsync(Patch.UpdateUserById(createUserResponseBody.Id), content);
            var responseStr = await patchResponse.Content.ReadAsStringAsync();
            Assert.True(patchResponse.StatusCode == HttpStatusCode.BadRequest);

            using (var scope = TestServerFixture.TestServer.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var updatedUser = await userManager.FindByIdAsync(createUserResponseBody.Id);
                Assert.True(updatedUser.Email == "foo@foo.com");
                Assert.True(updatedUser.UserName == "Foo");
            }
        }

        public void Dispose()
        {
            TestServerFixture.ClearUsers().Wait();
            UsersTestStartup.EmailSenderMock.Reset();
        }

        private HttpClient CreateClient()
        {
            var client = TestServerFixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Add("BypassAuthentication", "secret");
            return client;
        }
    }
}
