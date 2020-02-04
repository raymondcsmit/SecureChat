using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Specifications;
using Chat.API.Dtos;

namespace Chat.API.Application.Queries
{
    public interface IAssociationQueries
    {
        Task<FriendshipRequestDto> GetFriendshipRequestById(string id);
        Task<FriendshipRequestDto> GetFriendshipRequests(FriendshipRequestSpecification spec);
    }
}
