using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Dtos;
using Chat.API.Models;
using Helpers.Specifications;

namespace Chat.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserDto> GetUserByIdAsync(string id);

        Task<IReadOnlyList<UserDto>> GetUsersAsync(ISpecification<UserDto> spec);
    }
}
