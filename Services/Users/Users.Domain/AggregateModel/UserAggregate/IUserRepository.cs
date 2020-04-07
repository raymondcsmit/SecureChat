using System.Threading.Tasks;
using Users.Domain.SeedWork;

namespace Users.Domain.AggregateModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
