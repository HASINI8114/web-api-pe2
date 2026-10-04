using System;
using System.Configuration;
using System.Data.Common;
using GDB.Api.Infrastructure.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    public class DataBaseConnectionManager: IDataBaseConnectionManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataBaseConnectionManager> _logger;

        //private static IConfiguration _configuration;

        //public static void Initialize(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public DataBaseConnectionManager(IConfiguration configuration, ILogger<DataBaseConnectionManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public DbConnection GetConnection()
        {
            DataBaseProviderRegistration.Register(_configuration, _logger);

            string connectionString =
                _configuration.GetConnectionString("GDBConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError(
                    "Connection string 'GDBConnection' is missing.");

                throw new ConfigurationErrorsException(
                    "Connection string 'GDBConnection' not found.");
            }

            string providerName = _configuration["Database:ProviderName"];

            if (string.IsNullOrWhiteSpace(providerName))
            {
                _logger.LogError(
                    "Database provider name is missing.");

                throw new ConfigurationErrorsException(
                    "Database provider name not found.");
            }

            try
            {
                DbProviderFactory factory =
                    DbProviderFactories.GetFactory(providerName);

                DbConnection connection =
                    factory.CreateConnection();

                connection.ConnectionString =
                    connectionString;

                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create DB connection for provider {ProviderName}",
                    providerName);

                throw;
            }
        }
    }
}