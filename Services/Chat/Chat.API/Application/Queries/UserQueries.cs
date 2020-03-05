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
using Helpers.Specifications;
using Microsoft.Extensions.Options;
using Helpers.Specifications.Extensions;

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

        public async Task<(IEnumerable<UserDto>, int)> GetUsersAsync(ISpecification<UserDto> spec)
        {
            var baseSql = $@"SELECT * FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id;";
            var baseTotalSql = $@"SELECT COUNT(*) FROM Users";

            var (querySql, queryParam) = spec.Apply(baseSql);
            var (totalSql, totalParam) = spec.Apply(baseTotalSql, false);

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var results = await connection.QueryAsync<dynamic, dynamic, ProfileDto, UserDto>(querySql,
                    (u, _, p) => new UserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Profile = IsProfileEmpty(p) ? null : p
                    },
                    queryParam, splitOn: "userId,id");

                var count = await connection.QueryFirstAsync<int>(totalSql, totalParam);

                return (results, count);
            }
        }

        public async Task<(bool, bool)> UserNameOrEmailExists(string userName, string email)
        {
            var userNameQuery = userName == null
                ? @"(SELECT COUNT(*) FROM Users WHERE false)"
                : $@"(SELECT COUNT(*) FROM Users WHERE Users.UserName = @{nameof(userName)})";

            var emailQuery = email == null
                ? @"(SELECT COUNT(*) FROM Users WHERE false)"
                : $@"(SELECT COUNT(*) FROM Users WHERE Users.Email = @{nameof(email)})";

            var sql = string.Join("UNION ALL", userNameQuery, emailQuery);
            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<bool>(sql, new
                {
                    userName,
                    email
                });
                var resultList = result.ToList();
                return (resultList[0], resultList[1]);
            }
        }

        private bool IsProfileEmpty(ProfileDto profile)
        {
            return profile.Age == null && profile.Location == null && profile.Sex == null;
        }
    }
}
