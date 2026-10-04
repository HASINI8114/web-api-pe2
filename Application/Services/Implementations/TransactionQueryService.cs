using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Domain.Exceptions;
using GDB.Api.Domain.Models;
using GDB.Api.Infrastructure.Repositories.Contracts;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Application.Services.Implementations
{
    public class TransactionQueryService : ITransactionQueryService
    {
        private readonly string choice = "DB"; // This can be configured externally or via dependency injection
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionQueryService> _logger;

        public TransactionQueryService(
            IAccountRepositoryFactory accountRepositoryFactory,
            ITransactionRepositoryFactory transactionRepositoryFactory,
            ILogger<TransactionQueryService> logger)
        {
            _accountRepository = accountRepositoryFactory.Create(choice);
            _transactionRepository = transactionRepositoryFactory.Create(choice);
            _logger = logger;
        }

        public async Task<List<ViewRecentTransactionsResponseDto>> GetRecentTransactionsAsync(
            string accountNumber)
        {
            IAccount account =
                await _accountRepository.GetAccountAsync(accountNumber);

            if (account == null)
            {
                _logger.LogWarning(
                    "Recent transactions requested for unknown account {AccountNumber}",
                    accountNumber);

                throw new AccountException("Account not found");
            }

            List<ViewRecentTransactionsResponseDto> transactions =
                await _transactionRepository.GetRecentTransactionsAsync(accountNumber);

            _logger.LogInformation(
                "Fetched {Count} recent transactions for account {AccountNumber}",
                transactions.Count,
                accountNumber);

            return transactions;
        }
    }
}
