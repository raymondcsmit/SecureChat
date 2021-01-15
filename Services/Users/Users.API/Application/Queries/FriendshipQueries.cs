using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Users.API.Dtos;
using Users.Infrastructure;
using Dapper;
using Users.API.Application.Specifications;
using System.Linq;
using Users.API.Application.Specifications.Extensions;

namespace Users.API.Application.Queries
{
    public class FriendshipQueries : IFriendshipQueries
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;

        public FriendshipQueries(
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }

        public async Task<FriendshipDto> GetFriendshipById(string id)
        {
            var spec = new FriendshipSpecification(id);
            return (await GetFriendships(spec)).Item1.FirstOrDefault();
        }

        public async Task<(IEnumerable<FriendshipDto>, int)> GetFriendships(ISpecification<FriendshipDto> spec)
        {
            var totalSql = $@"SELECT COUNT(*) FROM Friendships";
            var (totalSpecSql, _) = spec.Apply(totalSql);

            var querySql = $@"SELECT Friendships.*, user1.*, p1.*, user2.*, p2.*
                        FROM Friendships
                        JOIN Users user1 ON Friendships.User1Id = user1.Id
                            LEFT JOIN UserProfileMap up1 ON up1.UserId = user1.Id
                                LEFT JOIN Profiles p1 ON p1.Id = up1.ProfileId
                        JOIN Users user2 ON Friendships.User2Id = user2.Id
                            LEFT JOIN UserProfileMap up2 ON up2.UserId = user2.Id
                                LEFT JOIN Profiles p2 ON p2.Id = up2.ProfileId;";
            var (querySpecSql, queryParam) = spec.Apply(querySql);

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var count = await connection.QuerySingleAsync<int>(totalSpecSql, queryParam);
                var results = await connection.QueryAsync<FriendshipDto, UserDto, ProfileDto, UserDto, ProfileDto, FriendshipDto>(querySpecSql,
                    (r, u1, p1, u2, p2) =>
                    {
                        u1.Profile = p1;
                        u2.Profile = p2;
                        r.User1 = u1;
                        r.User2 = u2;
                        return r;
                    },
                    queryParam, splitOn: "Id,Id,Id,Id");
                return (results, count);
            }
        }

        public async Task<(IEnumerable<FriendshipDto>, int)> GetFriendshipsByUserId(string userId, PaginationDto pagination = null)
        {
            var totalSql = $@"SELECT COUNT(*) FROM Friendships
                                WHERE Friendships.User1Id = @{nameof(userId)} OR Friendships.User2Id = @{nameof(userId)}";

            var querySql = $@"SELECT Friendships.*, user1.*, p1.*, user2.*, p2.*
                        FROM Friendships
                        JOIN Users user1 ON Friendships.User1Id = user1.Id
                            LEFT JOIN UserProfileMap up1 ON up1.UserId = user1.Id
                                LEFT JOIN Profiles p1 ON p1.Id = up1.ProfileId
                        JOIN Users user2 ON Friendships.User2Id = user2.Id
                            LEFT JOIN UserProfileMap up2 ON up2.UserId = user2.Id
                                LEFT JOIN Profiles p2 ON p2.Id = up2.ProfileId
                        WHERE user1.Id = @{nameof(userId)} OR user2.Id = @{nameof(userId)}";
            querySql += pagination != null
                ? $"\nLIMIT @{nameof(pagination.Limit)} OFFSET @{nameof(pagination.Offset)};"
                : ";";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var count = await connection.QuerySingleAsync<int>(totalSql, new { userId });
                var results = await connection.QueryAsync<FriendshipDto, UserDto, ProfileDto, UserDto, ProfileDto, FriendshipDto>(querySql,
                    (f, u1, p1, u2, p2) =>
                    {
                        u1.Profile = p1;
                        u2.Profile = p2;
                        f.User1 = u1;
                        f.User2 = u2;
                        return f;
                    },
                    new { userId, pagination?.Limit, pagination?.Offset },
                    splitOn: "Id,Id,Id,Id");
                return (results, count);
            }
        }
    }
}
