using System.Threading.Tasks;

namespace Chats.Domain.SeedWork
{
    public interface IRepository<T>
        where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        void Create(T friendship);

        Task<T> GetByIdAsync(string id);

        void DeleteById(string id);

        void Update(T entity);
    }
}
