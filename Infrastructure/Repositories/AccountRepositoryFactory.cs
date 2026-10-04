using GDB.Api.Domain.Enums;
using GDB.Api.Domain.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GDB.Api.Infrastructure.Repositories.Implementations;
using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Domain.Models;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    class AccountRepositoryFactory: IAccountRepositoryFactory
    {
        private readonly ILogger<AccountRepositoryFactory> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private readonly IDataBaseConnectionManager _connectionManager;
        private readonly IGDBInMemoryDataStore _gDBInMemoryDataStore;

        public AccountRepositoryFactory(IDataBaseConnectionManager connectionManager, IGDBInMemoryDataStore gDBInMemoryDataStore, ILoggerFactory loggerFactory)
        {
            _connectionManager = connectionManager;
            _gDBInMemoryDataStore = gDBInMemoryDataStore;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AccountRepositoryFactory>();
        }

        public IAccountRepository Create(string choice)
        {


            IAccountRepository repository =  null;


            if (choice.Equals("DB"))

                repository = new AccountRepositoryDB(_connectionManager, _loggerFactory.CreateLogger<AccountRepositoryDB>());

            else if (choice.Equals("InMemory"))

                repository = new AccountRepositoryInMemory(_gDBInMemoryDataStore);

            else
                _logger.LogWarning("Unknown account repository choice {Choice}; returning null", choice);

            return repository;

        }

    }
}







