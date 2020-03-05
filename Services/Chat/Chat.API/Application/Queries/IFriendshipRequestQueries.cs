using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Specifications;
using Chat.API.Dtos;
using Helpers.Specifications;

namespace Chat.API.Application.Queries
{
    public interface IFriendshipRequestQueries
    {
        Task<FriendshipRequestDto> GetFriendshipRequestById(string id);
        Task<(IEnumerable<FriendshipRequestDto>, int)> GetFriendshipRequests(ISpecification<FriendshipRequestDto> spec);
    }
}
