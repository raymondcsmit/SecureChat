using System;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Users.API.Infrastructure;

namespace Users.FunctionalTests
{
    public class TestServerFixture : IDisposable
    {
        public TestServer TestServer { get; set; }

        public void Dispose()
        {
            using (var scope = TestServer.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                dbContext.Database.EnsureDeleted();
            }
            TestServer?.Dispose();
            TestServer = null;
        }
    }
}
