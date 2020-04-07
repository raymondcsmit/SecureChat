using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Application.Specifications;
using Users.API.Dtos;
using Helpers.Specifications;

namespace Users.API.Application.Queries
{
    public interface IFriendshipRequestQueries
    {
        Task<FriendshipRequestDto> GetFriendshipRequestById(string id);
        Task<(IEnumerable<FriendshipRequestDto>, int)> GetFriendshipRequests(ISpecification<FriendshipRequestDto> spec);
    }
}
