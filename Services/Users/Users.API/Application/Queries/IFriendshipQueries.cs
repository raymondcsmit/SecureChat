using Users.API.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.API.Application.Specifications;

namespace Users.API.Application.Queries
{
    public interface IFriendshipQueries
    {
        Task<(IEnumerable<FriendshipDto>, int)> GetFriendships(ISpecification<FriendshipDto> spec);

        Task<(IEnumerable<FriendshipDto>, int)> GetFriendshipsByUserId(string userId, PaginationDto pagination = null);

        Task<FriendshipDto> GetFriendshipById(string id);
    }
}
