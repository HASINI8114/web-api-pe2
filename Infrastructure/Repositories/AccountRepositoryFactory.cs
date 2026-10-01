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
using gdb.Logging;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Infrastructure.Repositories
{
    class AccountRepositoryFactory
    {
        private static readonly ILogger _logger = AppLogger.CreateLogger<AccountRepositoryFactory>();



        public static IAccountRepository Create(string choice)
        {


            IAccountRepository repository =  null;


            if (choice.Equals("DB"))

                repository = new AccountRepositoryDB();

            else if (choice.Equals("InMemory"))

                repository = new AccountRepositoryInMemory();

            else
                _logger.LogWarning("Unknown account repository choice {Choice}; returning null", choice);

            return repository;

        }

    }
}







