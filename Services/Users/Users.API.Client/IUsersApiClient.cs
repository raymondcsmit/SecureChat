using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Users.API.Client.Dtos;

namespace Users.API.Client
{
    public interface IUsersApiClient
    {
        Task<IEnumerable<FriendshipDto>> GetFriendshipsByUserId(string id);
        Task<UserDto> GetUserById(string id);
    }
}
