using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Infrastructure.Repositories.Implementations;
using gdb.Logging;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    public static class TransactionRepositoryFactory
    {
        private static readonly ILogger _logger = AppLogger.CreateLogger("GDB.App.Infrastructure.Repositories.TransactionRepositoryFactory");

        public static ITransactionRepository Create(string type)
        {
            if (type == "DB")
            {
                return new TransactionRepositoryDB();
            }

            else if (type == "InMemory")
            {
                return new TransactionRepositoryInMemory();
            }

            _logger.LogError("Invalid transaction repository type {Type}", type);
            throw new Exception("Invalid transaction repository type");
        }
    }
}