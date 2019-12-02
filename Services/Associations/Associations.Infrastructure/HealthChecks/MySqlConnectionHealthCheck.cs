using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySql.Data.MySqlClient;

namespace Users.API.Infrastructure.HealthChecks
{
    public class MySqlConnectionHealthCheck : IHealthCheck
    {
        private const string DefaultTestQuery = "SELECT 1";

        private readonly string _connectionString;

        private readonly string _testQuery;

        public MySqlConnectionHealthCheck(string connectionString, string testQuery = DefaultTestQuery)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString)); ;
            _testQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    if (_testQuery != null)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = _testQuery;

                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (DbException ex)
                {
                    return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
                }
            }

            return HealthCheckResult.Healthy();
        }
    }

}
