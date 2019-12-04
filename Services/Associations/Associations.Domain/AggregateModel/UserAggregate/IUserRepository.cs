using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Associations.Domain.SeedWork;

namespace Associations.Domain.AggregateModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        User Add(User user);

        Task<User> GetAsync(string userId);
    }
}
