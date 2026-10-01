using System;
using System.Configuration;
using System.Data.Common;
using gdb.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    public class DataBaseProviderRegistration
    {
        private static readonly ILogger _logger =
            AppLogger.CreateLogger<DataBaseProviderRegistration>();

        public static void Register(IConfiguration configuration)
        {
            try
            {
                RegisterProvider(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to register DB provider factory");

                throw;
            }
        }

        private static void RegisterProvider(
            IConfiguration configuration)
        {
            string factoryTypeName =
                configuration["AppSettings:ProviderFactory"];

            if (string.IsNullOrWhiteSpace(factoryTypeName))
            {
                _logger.LogError(
                    "AppSetting 'ProviderFactory' is missing.");

                throw new ConfigurationErrorsException(
                    "AppSetting 'ProviderFactory' not found.");
            }

            Type factoryType =
                Type.GetType(factoryTypeName);

            if (factoryType == null)
            {
                throw new InvalidOperationException(
                    "Provider factory type not found: "
                    + factoryTypeName);
            }

            var instanceField =
                factoryType.GetField(
                    "Instance",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Static);

            if (instanceField == null)
            {
                throw new InvalidOperationException(
                    "No public static 'Instance' field on provider factory type: "
                    + factoryTypeName);
            }

            DbProviderFactory factory =
                (DbProviderFactory)
                instanceField.GetValue(null);

            DbProviderFactories.RegisterFactory(
                "Microsoft.Data.SqlClient",
                factory);

            _logger.LogInformation(
                "Registered DB provider {ProviderName}",
                "Microsoft.Data.SqlClient");
        }
    }
}