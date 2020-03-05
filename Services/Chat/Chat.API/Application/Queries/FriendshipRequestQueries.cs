using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chat.API.Dtos;
using Chat.Infrastructure;
using Dapper;
using Helpers.Specifications;
using Helpers.Specifications.Extensions;

namespace Chat.API.Application.Queries
{
    public class FriendshipRequestQueries: IFriendshipRequestQueries
    {
        private readonly IMapper _mapper;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IUserQueries _userQueries;

        public FriendshipRequestQueries(
            IMapper mapper,
            IDbConnectionFactory dbConnectionFactory,
            IUserQueries userQueries)
        {
            _mapper = mapper;
            _dbConnectionFactory = dbConnectionFactory;
            _userQueries = userQueries;
        }

        public async Task<FriendshipRequestDto> GetFriendshipRequestById(string id)
        {
            var sql = $@"SELECT * FROM FriendshipRequests
                                                WHERE FriendshipRequests.Id = @{nameof(id)};";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                return await connection.QueryFirstAsync<FriendshipRequestDto>(sql, new { id });
            }
        }

        public async Task<(IEnumerable<FriendshipRequestDto>, int)> GetFriendshipRequests(ISpecification<FriendshipRequestDto> spec)
        {
            var baseSql = $@"SELECT FriendshipRequests.*, user1.Id, user2.Id FROM FriendshipRequests
                        JOIN Users user1 ON FriendshipRequests.RequesterId = user1.Id
                        JOIN Users user2 ON FriendshipRequests.RequesteeId = user2.Id;";
            var baseTotalSql = $@"SELECT COUNT(*) FROM FriendshipRequests";

            var (querySql, queryParam) = spec.Apply(baseSql);
            var (totalSql, totalParam) = spec.Apply(baseTotalSql, false);

            var dict = new Dictionary<FriendshipRequestDto, (string, string)>();
            var count = 0;

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                await connection.QueryAsync<FriendshipRequestDto, string, string, FriendshipRequestDto>(querySql,
                    (req, uId1, uId2) =>
                    {
                        dict.Add(req, (uId1, uId2));
                        return req;
                    },
                    queryParam, splitOn: "user1.Id,user2.Id");

                count = await connection.QueryFirstAsync<int>(totalSql, totalParam);
            }


            foreach (var (req, ids) in dict)
            {
                req.Requester = await _userQueries.GetUserByIdAsync(ids.Item1);
                req.Requestee = await _userQueries.GetUserByIdAsync(ids.Item2); 
            }

            return (dict.Select((kvp) => kvp.Key), count);
        }
    }
}
