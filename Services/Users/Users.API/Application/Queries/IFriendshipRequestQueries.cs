using System.Collections.Generic;
using System.Threading.Tasks;
using Users.API.Application.Specifications;
using Users.API.Dtos;

namespace Users.API.Application.Queries
{
    public interface IFriendshipRequestQueries
    {
        Task<FriendshipRequestDto> GetFriendshipRequestById(string id);
        Task<(IEnumerable<FriendshipRequestDto>, int)> GetFriendshipRequests(ISpecification<FriendshipRequestDto> spec);
    }
}
