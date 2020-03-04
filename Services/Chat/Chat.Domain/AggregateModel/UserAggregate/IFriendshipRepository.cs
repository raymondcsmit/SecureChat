using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
    }
}
