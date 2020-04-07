using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.SeedWork;

namespace Users.Domain.AggregateModel.UserAggregate
{
    public interface IFriendshipRequestRepository : IRepository<FriendshipRequest>
    {
        Task<IEnumerable<FriendshipRequest>> GetByUserIdAsync(string userId);
    }
}
