using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Users.Domain.SeedWork;
using MediatR;

namespace Users.Infrastructure.UnitOfWork
{
    class OperationDescriptor
    {
        public Guid Id { get; } = Guid.NewGuid();
        public object Object { get; }
        public Func<IDbConnection, Task> Operation { get; }

        public bool IsDispatching =>
            Object.GetType().BaseType == typeof(Entity);

        public OperationDescriptor(object obj, Func<IDbConnection, Task> operation)
        {
            Object = obj;
            Operation = operation;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    // Not thread-safe
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly IMediator _mediator;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        private ConcurrentQueue<OperationDescriptor> _operations = new ConcurrentQueue<OperationDescriptor>();

        public SqlUnitOfWork(IMediator mediator, IDbConnectionFactory dbConnectionFactory)
        {
            _mediator = mediator;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                using (var conn = await _dbConnectionFactory.OpenConnectionAsync())
                using (var transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    if (!_operations.IsEmpty)
                    {
                        while (_operations.TryDequeue(out var operation))
                        {
                            await operation.Operation(conn).ConfigureAwait(false);
                            if (operation.IsDispatching)
                            {
                                await DispatchDomainEventsAsync(operation.Object as Entity).ConfigureAwait(false);
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
            finally
            {
                _operations = new ConcurrentQueue<OperationDescriptor>();
            }
        }

        public void AddOperation(object obj, Func<IDbConnection, Task> operation)
        {
            _operations.Enqueue(new OperationDescriptor(obj, operation));
        }

        public void AddOperation(Func<Task> operation)
        {
            _operations.Enqueue(new OperationDescriptor(new object(), _ => operation()));
        }

        private async Task DispatchDomainEventsAsync(Entity entity)
        {
            if (entity.DomainEvents.Any())
            {
                var tasks = entity.DomainEvents
                    .Select(async domainEvent => await _mediator.Publish(domainEvent));

                await Task.WhenAll(tasks);
                entity.ClearDomainEvents();
            }
        }
    }
}
