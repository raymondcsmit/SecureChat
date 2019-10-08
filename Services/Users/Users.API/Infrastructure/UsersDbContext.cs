using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
            var newEntities = ChangeTracker.Entries()
                .Where(
                    x => x.State == EntityState.Added && x.Entity is IAuditableModel
                )
                .Select(x => x.Entity as IAuditableModel);

            var modifiedEntities = ChangeTracker.Entries()
                .Where(
                    x => x.State == EntityState.Modified && x.Entity is IAuditableModel
                )
                .Select(x => x.Entity as IAuditableModel);

            foreach (var newEntity in newEntities)
            {
                newEntity.CreatedAt = DateTime.UtcNow;
                newEntity.LastModified = DateTime.UtcNow;
            }

            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.LastModified = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }
    }
}
