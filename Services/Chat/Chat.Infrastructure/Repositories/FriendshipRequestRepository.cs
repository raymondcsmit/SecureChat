using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Chat.Domain.AggregateModel.UserAggregate;
using Chat.Domain.SeedWork;
using Dapper;

namespace Chat.Infrastructure.Repositories
{
    public class FriendshipRequestRepository : IFriendshipRequestRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;
        public IUnitOfWork UnitOfWork { get; }

        public FriendshipRequestRepository(
            IUnitOfWork unitOfWork,
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FriendshipRequest>> GetByUserIdAsync(string userId)
        {
            var sql = $@"SELECT * FROM FriendshipRequests
                                                WHERE FriendshipRequests.RequesterId = @{nameof(userId)} OR FriendshipRequests.RequesteeId = @{nameof(userId)};";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                return await connection.QueryAsync<FriendshipRequest>(sql, new { userId });
            }
        }

        public void Create(FriendshipRequest request)
        {
            var sql = $@"INSERT INTO FriendshipRequests (RequesterId, RequesteeId, Outcome, CreatedAt, ModifiedAt)
                            VALUES (@{nameof(FriendshipRequest.RequesterId)}, 
                                    @{nameof(FriendshipRequest.RequesteeId)}, 
                                    @{nameof(FriendshipRequest.Outcome)}, 
                                    @{nameof(FriendshipRequest.CreatedAt)}, 
                                    @{nameof(FriendshipRequest.ModifiedAt)})";

            UnitOfWork.AddOperation(request, async connection =>
                await connection.ExecuteAsync(sql, request));
        }

        public async Task<FriendshipRequest> GetByIdAsync(string id)
        {
            var sql = $@"SELECT * FROM FriendshipRequests
                                                WHERE FriendshipRequests.RequesterId = @{nameof(id)};";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                return await connection.QueryFirstAsync<FriendshipRequest>(sql, new { id });
            }
        }

        public void DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(FriendshipRequest entity)
        {
            throw new NotImplementedException();
        }
    }
}
