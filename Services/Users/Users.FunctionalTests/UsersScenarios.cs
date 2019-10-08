using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Users.API.Infrastructure;
using Users.FunctionalTests.Extensions;
using Xunit;

namespace Users.FunctionalTests
{
    public class UsersScenarios : UsersScenarioBase, IClassFixture<TestServerFixture>
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
            using (var client = TestServerFixture.TestServer.CreateClient())
            {
                var user = new
                {
                    UserName = username,
                    Email = email,
                    Password = password
                };

                var response = await client.PostAsJsonAsync(Post.CreateUser, user);
                Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task CreateUser_ShouldCreateUser_OnValidData()
        {
            using (var client = TestServerFixture.TestServer.CreateClient())
            {
                var user = new
                {
                    UserName = "Foo",
                    Email = "foo@bar.com",
                    Password = "P@ssword1"
                };

                var response = await client.PostAsJsonAsync(Post.CreateUser, user);
                Assert.True(response.StatusCode == HttpStatusCode.Created);
                dynamic responseBody = await response.DeserializeAsync(user.GetType());
                Assert.Equal(user.UserName, responseBody.UserName);
            }
        }
    }
}
