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
        private readonly ILogger<TransactionRepositoryFactory> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public TransactionRepositoryFactory(IDataBaseConnectionManager connectionManager, IGDBInMemoryDataStore gDBInMemoryDataStore, IConfiguration configuration, ILogger<TransactionRepositoryFactory> logger, ILoggerFactory loggerFactory)
        {
            _connectionManager = connectionManager;
            _gDBInMemoryDataStore = gDBInMemoryDataStore;
            _choice = configuration["AppSettings:ConnectionChoice"];
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public ITransactionRepository Create()
        {
            if (_choice.Equals("DB"))
            {
                return new TransactionRepositoryDB(_connectionManager, _loggerFactory.CreateLogger<TransactionRepositoryDB>());
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