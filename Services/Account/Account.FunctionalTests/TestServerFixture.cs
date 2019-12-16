using System;
using System.Threading.Tasks;
using Account.API.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Account.FunctionalTests
{
    public class TestServerFixture: IDisposable
    {
        public TestServer TestServer { get; set; }

        public void Dispose()
        {
            DeleteDatabase();
            TestServer?.Dispose();
            TestServer = null;
        }

        public void DeleteDatabase()
        {
            using (var scope = TestServer?.Host.Services.CreateScope())
            {
                var dbContext = scope?.ServiceProvider.GetRequiredService<AccountDbContext>();
                dbContext?.Database.EnsureDeleted();
            }
        }

        public async Task ClearUsers()
        {
            using (var scope = TestServer.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
                await dbContext.Database.GetDbConnection().QueryAsync("DELETE FROM AspNetUsers;");
            }
        }
    }
}
