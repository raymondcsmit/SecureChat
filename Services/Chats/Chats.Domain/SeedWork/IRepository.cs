using System.Threading.Tasks;
using Chats.Domain.AggregateModel;

namespace Chats.Domain.SeedWork
{
    public interface IRepository<T>
        where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Chat Add(T chat);

        Task<T> GetAsync(string id);

        Task Delete(string id);

        Chat Update(T chat);
    }
}
