using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Users.API.Dtos;
using Users.API.Infrastructure.Exceptions;
using Users.Infrastructure;
using Dapper;
using Users.API.Application.Specifications;
using Users.API.Application.Specifications.Extensions;

namespace Users.API.Application.Queries
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
            var sql = $@"SELECT Users.*, Profiles.* 
                        FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id
                        WHERE Users.Id = @{nameof(id)}
                        LIMIT 1";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var results = await connection.QueryAsync<UserDto, ProfileDto, UserDto>(sql,
                    (u, p) =>
                    {
                        u.Profile = p;
                        return u;
                    },
                    new { id }, splitOn: "id");

                return results.FirstOrDefault()
                           ?? throw new ChatApiException("Could not fetch user", new[] { "User not found" }, 404);
            }
        }

        public async Task<(IEnumerable<UserDto>, int)> GetUsersAsync(ISpecification<UserDto> spec)
        {
            var totalSql = $@"SELECT COUNT(*) as count FROM Users";

            var querySql = $@"SELECT Users.*, Profiles.*
                        FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id;";

            var (querySpecSql, queryParam) = spec.Apply(querySql);

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var count = await connection.QuerySingleAsync<int>(totalSql, queryParam);
                var results = await connection.QueryAsync<UserDto, ProfileDto, UserDto>(querySpecSql,
                    (u, p) =>
                    {
                        u.Profile = p;
                        return u;
                    },
                    queryParam, splitOn: "id");

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
    }
}
