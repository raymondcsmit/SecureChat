using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Chat.Domain.SeedWork;
using Dapper;

namespace Chat.Infrastructure.Repositories
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
            var sql = $@"INSERT INTO Friendships (Id, UserId1, UserId2)
                              VALUES (@{nameof(Friendship.UserId1)},
                                      @{nameof(Friendship.UserId2)});";

            UnitOfWork.AddOperation(friendship, async connection =>
                await connection.ExecuteAsync(sql, friendship));
        }

        public async Task<Friendship> GetByIdAsync(string id)
        {
            var sql = $@"SELECT * FROM Friendships
                                WHERE Friendships.Id = @{nameof(id)};";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                return await connection.QueryFirstAsync<Friendship>(sql, new { id });
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
    }
}
