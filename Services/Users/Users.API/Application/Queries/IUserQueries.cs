using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Dtos;
using Users.API.Models;
using Helpers.Specifications;

namespace Users.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserDto> GetUserByIdAsync(string id);

        Task<(IEnumerable<UserDto>, int)> GetUsersAsync(ISpecification<UserDto> spec);

        Task<(bool userNameExists, bool emailExists)> UserNameOrEmailExists(string userName, string email);
    }
}
