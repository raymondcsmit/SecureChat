using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.AggregateModel.UserAggregate;
using Users.Domain.SeedWork;
using Dapper;

namespace Users.Infrastructure.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public IUnitOfWork UnitOfWork { get; }

        public FriendshipRepository(
            IUnitOfWork unitOfWork,
            IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            UnitOfWork = unitOfWork;
        }

        public void Create(Friendship friendship)
        {
            var sql = $@"INSERT INTO Friendships (User1Id, User2Id)
                              VALUES (@{nameof(Friendship.User1Id)},
                                      @{nameof(Friendship.User2Id)});";

            UnitOfWork.AddOperation(friendship, async connection =>
                await connection.ExecuteAsync(sql, friendship));
        }

        public async Task<Friendship> GetByIdAsync(string id)
        {
            var sql = $@"SELECT * FROM Friendships
                                WHERE Friendships.Id = @{nameof(id)};";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Friendship>(sql, new { id });
            }
        }

        public void DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Friendship entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Friendship>> GetByUserIdAsync(string id)
        {
            var sql = $@"SELECT * FROM Friendships
                                WHERE Friendships.User1Id = @{nameof(id)} OR Friendships.User2Id = @{nameof(id)};";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                return await connection.QueryAsync<Friendship>(sql, new { id });
            }
        }
    }
}
