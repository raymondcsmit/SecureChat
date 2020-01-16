using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Dtos;
using Chat.API.Models;

namespace Chat.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserDto> GetUserByIdAsync(string id);

        Task<ArrayResponse<UserDto>> GetUsersAsync(UserQuery userQuery, Pagination pagination);
    }
}
