using System.Collections.Generic;
using System.Threading.Tasks;
using Users.API.Application.Specifications;
using Users.API.Dtos;

namespace Users.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserDto> GetUserByIdAsync(string id);

        Task<(IEnumerable<UserDto>, int)> GetUsersAsync(ISpecification<UserDto> spec);

        Task<(bool userNameExists, bool emailExists)> UserNameOrEmailExists(string userName, string email);
    }
}
