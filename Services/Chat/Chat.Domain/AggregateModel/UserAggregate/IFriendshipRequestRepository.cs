using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public interface IFriendshipRequestRepository : IRepository<FriendshipRequest>
    {
        Task<IEnumerable<FriendshipRequest>> GetByUserIdAsync(string userId);
    }
}
