using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Users.API.Infrastructure;

namespace Users.FunctionalTests
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
                var dbContext = scope?.ServiceProvider.GetRequiredService<UsersDbContext>();
                dbContext?.Database.EnsureDeleted();
            }
        }

        public async Task ClearUsers()
        {
            using (var scope = TestServer.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await dbContext.Database.GetDbConnection().QueryAsync("DELETE FROM AspNetUsers;");
            }
        }
    }
}
