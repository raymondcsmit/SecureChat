using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Application.Specifications;
using Users.API.Dtos;
using Users.Infrastructure;
using Dapper;
using Users.API.Application.Specifications.Extensions;

namespace Users.API.Application.Queries
{
    public class FriendshipRequestQueries: IFriendshipRequestQueries
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public FriendshipRequestQueries(
            IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<FriendshipRequestDto> GetFriendshipRequestById(string id)
        {
            var spec = new FriendshipRequestSpecification(id);
            return (await GetFriendshipRequests(spec)).Item1.FirstOrDefault();
        }

        public async Task<(IEnumerable<FriendshipRequestDto>, int)> GetFriendshipRequests(ISpecification<FriendshipRequestDto> spec)
        {
            var totalSql = $@"SELECT COUNT(*) FROM FriendshipRequests";
            var (totalSpecSql, _) = spec.Apply(totalSql);

            var querySql = $@"SELECT FriendshipRequests.*, user1.*, p1.*, user2.*, p2.*
                        FROM FriendshipRequests
                        JOIN Users user1 ON FriendshipRequests.RequesterId = user1.Id
                            LEFT JOIN UserProfileMap up1 ON up1.UserId = user1.Id
                                LEFT JOIN Profiles p1 ON p1.Id = up1.ProfileId
                        JOIN Users user2 ON FriendshipRequests.RequesteeId = user2.Id
                            LEFT JOIN UserProfileMap up2 ON up2.UserId = user2.Id
                                LEFT JOIN Profiles p2 ON p2.Id = up2.ProfileId;";
            var (querySpecSql, queryParam) = spec.Apply(querySql);

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var count = await connection.QuerySingleAsync<int>(totalSpecSql, queryParam);
                var results = await connection.QueryAsync<FriendshipRequestDto, UserDto, ProfileDto, UserDto, ProfileDto, FriendshipRequestDto>(querySpecSql,
                    (r, u1, p1, u2, p2) =>
                    {
                        u1.Profile = p1;
                        u2.Profile = p2;
                        r.Requester = u1;
                        r.Requestee = u2;
                        return r;
                    },
                    queryParam, splitOn: "Id,Id,Id,Id");
                return (results, count);
            }
        }
    }
}
