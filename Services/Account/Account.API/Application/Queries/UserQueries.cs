using System.Linq;
using System.Threading.Tasks;
using Account.API.Dtos;
using Account.API.Infrastructure;
using Account.API.Infrastructure.Exceptions;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Account.API.Application.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly IMapper _mapper;
        private DbConnectionInfo _dbConnectionInfo;

        public UserQueries(
            IMapper mapper, 
            IOptions<DbConnectionInfo> dbConnectionInfo)
        {
            _mapper = mapper;
            _dbConnectionInfo = dbConnectionInfo.Value;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            using (var connection = new MySqlConnection(_dbConnectionInfo.ConnectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<dynamic>(
                    @"SELECT *
                         FROM AspNetUsers AS users 
                         WHERE users.Id = @Id
                         LIMIT 1",
                    new { Id = id }
                );

                var user = result.FirstOrDefault() 
                           ?? throw new AccountApiException("Could not fetch user", new[] { "User not found" }, 404);

                return _mapper.Map<UserDto>(user);
            }
        }
    }
}
