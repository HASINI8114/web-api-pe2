using gdb.Logging;
using GDB.Api.Domain;
using GDB.Api.Domain.Enums;
using GDB.Api.Domain.Exceptions;
using GDB.Api.Domain.Models;
using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Infrastructure.Repositories.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Infrastructure.Repositories
{
    class AccountRepositoryFactory: IAccountRepositoryFactory
    {
        private static readonly ILogger _logger = AppLogger.CreateLogger<AccountRepositoryFactory>();

        private readonly IDataBaseConnectionManager _connectionManager;
        private readonly IGDBInMemoryDataStore _gDBInMemoryDataStore;
        private readonly IAccountFactory _accountFactory;
        private readonly string _choice;

        public AccountRepositoryFactory(IDataBaseConnectionManager connectionManager, IGDBInMemoryDataStore gDBInMemoryDataStore, IAccountFactory accountFactory, IConfiguration configuration)
        {
            _connectionManager = connectionManager;
            _gDBInMemoryDataStore = gDBInMemoryDataStore;
            _accountFactory = accountFactory;
            _choice = configuration["AppSettings:ConnectionChoice"];
        }

        public IAccountRepository Create()
        {


            IAccountRepository repository =  null;


            if (_choice.Equals("DB"))

                repository = new AccountRepositoryDB(_connectionManager);

            else if (_choice.Equals("InMemory"))

                repository = new AccountRepositoryInMemory(_gDBInMemoryDataStore, _accountFactory);

            else
                _logger.LogWarning("Unknown account repository choice {Choice}; returning null", _choice);

            return repository;

        }

    }
}







