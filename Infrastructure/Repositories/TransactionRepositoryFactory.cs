using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Infrastructure.Repositories.Implementations;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    public class TransactionRepositoryFactory : ITransactionRepositoryFactory
    {

        private readonly IDataBaseConnectionManager _connectionManager;
        private readonly IGDBInMemoryDataStore _gDBInMemoryDataStore;
        private readonly string _choice;
        public TransactionRepositoryFactory(IDataBaseConnectionManager connectionManager, IGDBInMemoryDataStore gDBInMemoryDataStore, IConfiguration configuration)
        {
            _connectionManager = connectionManager;
            _gDBInMemoryDataStore = gDBInMemoryDataStore;
            _choice = configuration["AppSettings:ConnectionChoice"];
        }
        private static readonly ILogger _logger = AppLogger.CreateLogger("GDB.App.Infrastructure.Repositories.TransactionRepositoryFactory");
        public ITransactionRepository Create()
        {
            if (_choice.Equals("DB"))
            {
                return new TransactionRepositoryDB(_connectionManager);
            }

            else if (_choice.Equals("InMemory"))
            {
                return new TransactionRepositoryInMemory(_gDBInMemoryDataStore);
            }
            else 
            {
                _logger.LogError("Invalid transaction repository choice {Choice}", _choice);
                throw new Exception("Invalid transaction repository type");
            }
            
        }
    }
}