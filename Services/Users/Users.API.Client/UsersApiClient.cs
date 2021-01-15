using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Users.API.Client.Dtos;

namespace Users.API.Client
{
    public class UsersApiClient: IUsersApiClient
    {
        private readonly HttpClient _httpClient;

        public UsersApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<FriendshipDto>> GetFriendshipsByUserId(string id)
        {
            var uri = $"api/users/{id}/friendships";

            var responseString = await _httpClient.GetStringAsync(uri);
            var friendships = JsonConvert.DeserializeObject<ArrayResponse<FriendshipDto>>(responseString);
            return friendships.Items;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var uri = $"api/users/{id}";

            var responseString = await _httpClient.GetStringAsync(uri);
            var user = JsonConvert.DeserializeObject<UserDto>(responseString);
            return user;
        }
    }
}
