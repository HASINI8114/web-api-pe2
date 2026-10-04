using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Infrastructure.Repositories.Implementations;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    public class TransactionRepositoryFactory : ITransactionRepositoryFactory
    {

        private readonly IDataBaseConnectionManager _connectionManager;
        private readonly IGDBInMemoryDataStore _gDBInMemoryDataStore;
        private readonly ILogger<TransactionRepositoryFactory> _logger;

        public TransactionRepositoryFactory(IDataBaseConnectionManager connectionManager, IGDBInMemoryDataStore gDBInMemoryDataStore, ILogger<TransactionRepositoryFactory> logger)
        {
            _connectionManager = connectionManager;
            _gDBInMemoryDataStore = gDBInMemoryDataStore;
            _logger = logger;
        }

        public ITransactionRepository Create(string type)
        {
            if (type == "DB")
            {
                return new TransactionRepositoryDB(_connectionManager);
            }

            else if (type == "InMemory")
            {
                return new TransactionRepositoryInMemory(_gDBInMemoryDataStore);
            }

            _logger.LogError("Invalid transaction repository type {Type}", type);
            throw new Exception("Invalid transaction repository type");
        }
    }
}