using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chat.API.Dtos;
using Chat.API.Infrastructure.Exceptions;
using Chat.API.Models;
using Chat.Infrastructure;
using Dapper;
using Microsoft.Extensions.Options;

namespace Chat.API.Application.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;

        public UserQueries(
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var sql = $@"SELECT * FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id
                        WHERE Users.Id = @{nameof(id)}
                        LIMIT 1";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var results = await connection.QueryAsync<dynamic, dynamic, ProfileDto, UserDto>(sql,
                    (u, _, p) => new UserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Profile = IsProfileEmpty(p) ? null : p
                    },
                    new { id }, splitOn: "userId,id"); ;

                return results.FirstOrDefault()
                           ?? throw new ChatApiException("Could not fetch user", new[] { "User not found" }, 404);
            }
        }

        public async Task<ArrayResponse<UserDto>> GetUsersAsync(UserQuery userQuery, Pagination pagination)
        {
            throw new NotImplementedException();
        }

        private bool IsProfileEmpty(ProfileDto profile)
        {
            return profile.Age == null && profile.Location == null && profile.Sex == null;
        }
    }
}
