using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users.API.Models;

namespace Users.API.Infrastructure
{
    public class UsersDbContext: IdentityDbContext<User>
    {
        private readonly ILoggerFactory _loggerFactory;

        public UsersDbContext(DbContextOptions<UsersDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
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
}
