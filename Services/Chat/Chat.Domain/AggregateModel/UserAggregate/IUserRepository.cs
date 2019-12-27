using System.Threading.Tasks;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        void Add(User user);

        void Update(User user);

        Task<User> GetAsync(string userId);

        Task<(bool, bool)> UserNameOrEmailExists(string userName, string email);
    }
}
