using System;
using System.Data;
using System.Threading.Tasks;
using Users.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Users.FunctionalTests
{
    public class TestServerFixture: IDisposable
    {
        public TestServer TestServer { get; set; }

        public void Dispose()
        {
            ClearUsers().Wait();
            TestServer?.Dispose();
            TestServer = null;
        }

        public async Task ClearUsers()
        {
            using (var scope = TestServer.Host.Services.CreateScope())
            {
                var conn = await scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>().OpenConnectionAsync();
                await conn.QueryAsync("DELETE FROM UserProfileMap; DELETE FROM Users; DELETE FROM Profiles");
            }
        }
    }
}
