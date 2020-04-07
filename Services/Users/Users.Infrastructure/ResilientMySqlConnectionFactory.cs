using System.Data;
using System.Threading.Tasks;
using Helpers.Resilience;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Users.Infrastructure
{
    public class ResilientMySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IDatabaseResiliencePolicy _policy;
        private readonly DbConnectionInfo _connectionInfo;

        public ResilientMySqlConnectionFactory(IOptions<DbConnectionInfo> connectionInfo, IDatabaseResiliencePolicy policy)
        {
            _policy = policy;
            _connectionInfo = connectionInfo.Value;
        }

        public async Task<IDbConnection> OpenConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionInfo.ConnectionString);
            await _policy.ExecuteAsync(async () => await connection.OpenAsync());
            return connection;
        }
    }
}
