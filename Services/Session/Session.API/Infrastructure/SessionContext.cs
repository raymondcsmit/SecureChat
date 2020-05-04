using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Session.API.EntityConfigurations;
using Session.API.Model;
using Session.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Session.API.Infrastructure
{
    public class SessionContext : DbContext
    {
        public SessionContext(DbContextOptions<SessionContext> options) : base(options)
        {}

        public DbSet<ChatUser> ChatUsers { get; set; }

        public DbSet<ChatEvent> ChatEvents { get; set; }

        public DbSet<ChatUserEvent> ChatUserEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ChatUserEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChatEventEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChatUserEventEntityTypeConfiguration());
        }

        public override int SaveChanges()
        {
            SetAuditTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            SetAuditTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditTimestamps()
        {
            var newEntities = ChangeTracker.Entries()
                .Where(
                    x => x.State == EntityState.Added && x.Entity is IAuditable
                )
                .Select(x => x.Entity as IAuditable);

            var modifiedEntities = ChangeTracker.Entries()
                .Where(
                    x => x.State == EntityState.Modified && x.Entity is IAuditable
                )
                .Select(x => x.Entity as IAuditable);

            foreach (var newEntity in newEntities)
            {
                newEntity.CreatedAt = DateTimeOffset.Now;
                newEntity.ModifiedAt = DateTimeOffset.Now;
            }

            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.ModifiedAt = DateTimeOffset.Now;
            }
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
