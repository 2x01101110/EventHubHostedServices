using EventHubHostedServices.BuildingBlocks.Contracts.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;

namespace EventHubHostedServices.BuildingBlocks.Infrastructure
{
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private ILogger<SqlConnectionFactory> _logger;
        private readonly string _connectionString;
        private IDbConnection _connection;

        public SqlConnectionFactory(
            ILogger<SqlConnectionFactory> logger,
            IConfiguration configuration)
        {
            this._connectionString = configuration["SqlConnectionString"];
            this._logger = logger;
        }

        public IDbConnection GetOpenConnection()
        {
            try
            {
                if (this._connection == null || this._connection.State != ConnectionState.Open)
                {
                    this._connection = new SqlConnection(this._connectionString);
                    this._connection.Open();
                }

                return this._connection;
            }
            catch (Exception ex)
            {
                this._logger.LogError($"[{DateTime.Now}] {ex.Message} {ex.StackTrace}");
                throw ex;
            }
        }

        public void Dispose()
        {
            if (this._connection != null && this._connection.State == ConnectionState.Open)
            {
                this._connection.Dispose();
            }
        }
    }
}
