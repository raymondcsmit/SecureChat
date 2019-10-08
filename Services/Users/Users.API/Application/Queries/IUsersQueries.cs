using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Dtos;

namespace Users.API.Application.Queries
{
    public interface IUsersQueries
    {
        Task<UserDto> GetUserByIdAsync(string id);
    }
}
