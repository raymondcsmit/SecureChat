using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chats.Domain.AggregateModel;
using Chats.Domain.SeedWork;
using Chats.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Chats.Infrastructure
{
    public class ChatsContext: DbContext, IUnitOfWork
    {
        private readonly DbContextOptions<ChatsContext> _options;
        private readonly List<Func<Task>> _operations = new List<Func<Task>>();

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMembership> ChatMemberships { get; set; }
        public DbSet<ChatModerator> ChatModerators { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        public ChatsContext(DbContextOptions<ChatsContext> options, IMediator mediator) : base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ChatEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChatMembershipEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChatModeratorEntityTypeConfiguration());
            builder.ApplyConfiguration(new MessageEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            using (var transaction = await Database.BeginTransactionAsync(cancellationToken))
            {
                SetAuditTimestamps();
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess: false, cancellationToken);
                await ExecuteExternalOperations();
                ChangeTracker.AcceptAllChanges();
                await transaction.CommitAsync(cancellationToken);
                return result;
            }

        }

        public void AddOperation(Func<Task> operation)
        {
            _operations.Add(operation);
        }

        private async Task ExecuteExternalOperations()
        {
            foreach (var operation in _operations)
            {
                await operation().ConfigureAwait(false);
            }
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

    public class ChatsContextDesignFactory : IDesignTimeDbContextFactory<ChatsContext>
    {
        public ChatsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChatsContext>()
                .UseMySql("Server=mysql; Database=chats_db; Uid=foo; Pwd=bar");

            return new ChatsContext(optionsBuilder.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public async Task<object> Send(object request, CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

        }
    }
}