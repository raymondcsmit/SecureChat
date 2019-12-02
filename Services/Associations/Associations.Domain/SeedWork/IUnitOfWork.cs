using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Associations.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        void AddOperation(object entity, Func<IDbConnection, Task> operation);
    }
}
