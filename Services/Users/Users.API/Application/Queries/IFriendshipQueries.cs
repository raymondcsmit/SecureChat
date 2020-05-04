using Users.API.Dtos;
using Helpers.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Application.Queries
{
    public interface IFriendshipQueries
    {
        Task<(IEnumerable<FriendshipDto>, int)> GetFriendships(ISpecification<FriendshipDto> spec);

        Task<(IEnumerable<FriendshipDto>, int)> GetFriendshipsByUserId(string userId, PaginationDto pagination = null);

        Task<FriendshipDto> GetFriendshipById(string id);
    }
}
