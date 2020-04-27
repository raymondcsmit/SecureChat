using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Session.API.EntityConfigurations;
using Session.API.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.Infrastructure
{
    public class SessionContext : DbContext
    {
        public SessionContext(DbContextOptions<SessionContext> options) : base(options)
        {}

        public DbSet<ChatSession> ChatSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ChatSessonEntityTypeConfiguration());
        }
    }

    public class SessionContextDesignFactory : IDesignTimeDbContextFactory<SessionContext>
    {
        public SessionContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["ConnectionString"];
            var optionsBuilder = new DbContextOptionsBuilder<SessionContext>()
                .UseMySql(connectionString);
            return new SessionContext(optionsBuilder.Options);
        }
    }
}
