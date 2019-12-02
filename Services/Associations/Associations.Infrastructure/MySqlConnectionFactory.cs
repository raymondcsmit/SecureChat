using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Associations.Infrastructure
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly DbConnectionInfo _connectionInfo;

        public MySqlConnectionFactory(IOptions<DbConnectionInfo> connectionInfo)
        {
            _connectionInfo = connectionInfo.Value;
        }

        public async Task<IDbConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionInfo.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
