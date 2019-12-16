using System;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Chat.Domain.SeedWork;
using Dapper;

namespace Chat.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public IUnitOfWork UnitOfWork { get; }

        public UserRepository(IUnitOfWork unitOfWork, IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            UnitOfWork = unitOfWork;
        }

        public void Add(User user)
        {
            var sql = $@"INSERT INTO Users (Id, UserName)
                        VALUES (@{nameof(user.Id)}, @{nameof(user.UserName)});";
            UnitOfWork.AddOperation(user, async connection =>
            {
                await connection.ExecuteAsync(sql, new
                {
                    user.Id,
                    Username = user.UserName
                });
            });
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
