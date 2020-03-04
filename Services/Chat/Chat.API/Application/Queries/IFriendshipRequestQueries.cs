using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Specifications;

namespace Chat.API.Application.Queries
{
    public interface IFriendshipRequestQueries
    {
        Task<object> GetFriendshipRequestById(string id);
        Task<object> GetFriendshipRequests(FriendshipRequestSpecification spec);
    }
}
