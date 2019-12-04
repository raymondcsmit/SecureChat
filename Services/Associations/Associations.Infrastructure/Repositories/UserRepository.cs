using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Associations.Domain.AggregateModel.UserAggregate;
using Associations.Domain.SeedWork;

namespace Associations.Infrastructure.Repositories
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

        public User Add(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
