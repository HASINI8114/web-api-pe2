using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Domain.Exceptions;
using GDB.Api.Domain.Models;
using GDB.Api.Infrastructure.Repositories;
using GDB.Api.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Services.Implementations
{
    public class TransactionQueryService
        : ITransactionQueryService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionQueryService()
        {
            _accountRepository =
                AccountRepositoryFactory.Create("DB");

            _transactionRepository =
                TransactionRepositoryFactory.Create("DB");
        }


        public async Task<List<ViewRecentTransactionsResponseDto>>
            GetRecentTransactionsAsync(
                string accountNumber)
        {
            IAccount account =
                await _accountRepository.GetAccountAsync(
                    accountNumber);

            if (account == null)
            {
                throw new AccountException(
                    "Account not found.");
            }

            return _transactionRepository
                .GetRecentTransactions(accountNumber);
        }
    }
}
