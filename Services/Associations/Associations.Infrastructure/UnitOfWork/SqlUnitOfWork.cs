using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Associations.Domain.SeedWork;
using MediatR;

namespace Associations.Infrastructure.UnitOfWork
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

        private List<OperationDescriptor> _operations = new List<OperationDescriptor>();

        public SqlUnitOfWork(IMediator mediator, IDbConnectionFactory dbConnectionFactory)
        {
            _mediator = mediator;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                using (var conn = await _dbConnectionFactory.GetConnectionAsync())
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var operation in _operations)
                    {
                        await operation.Operation(conn);
                        if (operation.IsDispatching)
                        {
                            await DispatchDomainEventsAsync(operation.Object as Entity);
                        }
                    }

                    transaction.Commit();
                }
            }
            finally
            {
                _operations.Clear();
            }
        }

        public void AddOperation(object obj, Func<IDbConnection, Task> operation)
        {
            _operations.Add(new OperationDescriptor(obj, operation));
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
