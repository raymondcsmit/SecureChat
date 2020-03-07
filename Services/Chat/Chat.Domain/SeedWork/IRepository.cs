using System.Threading.Tasks;

namespace Chat.Domain.SeedWork
{
    public interface IRepository<T>
        where T : Entity
    {
        IUnitOfWork UnitOfWork { get; }

        void Create(T friendship);

        Task<T> GetByIdAsync(string id);

        void DeleteById(string id);

        void Update(T entity);
    }
}
