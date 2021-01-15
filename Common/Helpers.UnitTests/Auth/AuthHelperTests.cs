using Helpers.Auth;
using Helpers.Auth.AuthHelper;
using Moq;
using Xunit;

namespace Helpers.UnitTests.Auth
{
    public class AuthHelperTests
    {
        [Fact]
        public void AllowSystem_ShouldReturnTrue_IfUserIsSystem()
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .AllowId("gibberish")
                .AllowPermissions("gibberish1", "gibberish2")
                .Build();

            var mock = new Mock<IIdentityService>();
            mock.Setup(idS => idS.GetPermissions()).Returns(new[] {"foo", "bar "});
            mock.Setup(idS => idS.GetUserIdentity()).Returns(AuthorizationConstants.System);

            var authorized = authHelper.Authorize(mock.Object);
            Assert.True(authorized);
        }

        [Fact]
        public void AllowSystem_ShouldDefer_IfUserIsNotSystem()
        {
            var authHelper = new AuthHelperBuilder()
                .AllowSystem()
                .AllowId("baz")
                .AllowPermissions("gibberish1", "gibberish2")
                .Build();

            var mock = new Mock<IIdentityService>();
            mock.Setup(idS => idS.GetPermissions()).Returns(new[] { "foo", "bar" });
            mock.Setup(idS => idS.GetUserIdentity()).Returns("baz");

            var authorized = authHelper.Authorize(mock.Object);
            Assert.True(authorized);
        }

        [Fact]
        public void AllowId_ShouldReturnTrue_IfUserHasId()
        {
            var authHelper = new AuthHelperBuilder()
                .AllowId("baz")
                .AllowPermissions("gibberish1", "gibberish2")
                .Build();

            var mock = new Mock<IIdentityService>();
            mock.Setup(idS => idS.GetPermissions()).Returns(new[] { "foo", "bar" });
            mock.Setup(idS => idS.GetUserIdentity()).Returns("baz");

            var authorized = authHelper.Authorize(mock.Object);
            Assert.True(authorized);
        }

        [Fact]
        public void AllowId_ShouldDefer_IfUserDoesNotHaveId()
        {
            var authHelper = new AuthHelperBuilder()
                .AllowId("baz")
                .AllowPermissions("foo", "bar")
                .Build();

            var mock = new Mock<IIdentityService>();
            mock.Setup(idS => idS.GetPermissions()).Returns(new[] { "foo", "bar" });
            mock.Setup(idS => idS.GetUserIdentity()).Returns("baz");

            var authorized = authHelper.Authorize(mock.Object);
            Assert.True(authorized);
        }

        [Fact]
        public void RequirePermissions_ShouldDefer_IfUserHasPermissions()
        {
            var authHelper = new AuthHelperBuilder()
                .AllowPermissions("foo", "bar")
                .AllowId("baz")
                .Build();

            var mock = new Mock<IIdentityService>();
            mock.Setup(idS => idS.GetPermissions()).Returns(new[] { "foo", "bar" });
            mock.Setup(idS => idS.GetUserIdentity()).Returns("baz");

            var authorized = authHelper.Authorize(mock.Object);
            Assert.True(authorized);
        }

        [Fact]
        public void RequirePermissions_ShouldReturnFalse_IfUserDoesNotHavePermissions()
        {
            var authHelper = new AuthHelperBuilder()
                .AllowPermissions("foo", "bar")
                .AllowId("baz")
                .Build();

            var mock = new Mock<IIdentityService>();
            mock.Setup(idS => idS.GetPermissions()).Returns(new[] { "foo", "gibberish1", "gibberish2" });
            mock.Setup(idS => idS.GetUserIdentity()).Returns("baz");

            var authorized = authHelper.Authorize(mock.Object);
            Assert.False(authorized);
        }
    }
}
