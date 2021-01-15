using Chats.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Domain.SeedWork
{
    public interface IGenericRepository<T> : IRepository<T>
        where T : IAggregateRoot
    {
        Task<(ICollection<T>, int)> GetAsync(ISpecification<T> specification);
    }
}
