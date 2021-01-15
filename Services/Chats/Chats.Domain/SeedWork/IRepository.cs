using System.Threading.Tasks;
using Chats.Domain.AggregateModel;

namespace Chats.Domain.SeedWork
{
    public interface IRepository<T>
        where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        T Create(T entity);

        Task<T> GetAsync(string id);

        Task DeleteAsync(string id);

        T Update(T entity);
    }
}
