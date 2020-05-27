using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.AggregateModel.UserAggregate;
using Users.Domain.SeedWork;
using Dapper;
using AutoMapper;

namespace Users.Infrastructure.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;

        public IUnitOfWork UnitOfWork { get; }

        public FriendshipRepository(
            IUnitOfWork unitOfWork,
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
            UnitOfWork = unitOfWork;
        }

        public void Create(Friendship friendship)
        {
            var sql = $@"INSERT INTO Friendships (User1Id, User2Id)
                              VALUES (@{nameof(Friendship.User1Id)},
                                      @{nameof(Friendship.User2Id)});
                         SELECT * 
                         FROM Friendships f
                         WHERE f.Id = LAST_INSERT_ID();";

            UnitOfWork.AddOperation(friendship, async connection =>
            {
                var createdFriendship = await connection.QueryFirstAsync<Friendship>(sql, friendship);
                _mapper.Map(createdFriendship, friendship);
            });
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
            var sql = $@"DELETE FROM Friendships
                                WHERE Friendships.Id = @{nameof(id)};";

            UnitOfWork.AddOperation(new object(), async connection => await connection.ExecuteAsync(sql, new {id}));
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
