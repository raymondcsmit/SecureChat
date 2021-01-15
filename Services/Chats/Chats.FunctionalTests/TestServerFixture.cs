using System;
using System.Threading.Tasks;
using Chats.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chats.FunctionalTests
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
                var dbContext = scope?.ServiceProvider.GetRequiredService<ChatsContext>();
                dbContext?.Database.EnsureDeleted();
            }
        }

        public async Task ClearChats()
        {
            using (var scope = TestServer?.Host.Services.CreateScope())
            {
                var dbContext = scope?.ServiceProvider.GetRequiredService<ChatsContext>();
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Chats;");
            }
        }
    }
}
