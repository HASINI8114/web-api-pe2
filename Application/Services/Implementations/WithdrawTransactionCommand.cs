using GDB.Api.Application.Dtos.Request;
using GDB.Api.Application.Dtos.Response;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Domain.Enums;
using GDB.Api.Domain.Exceptions;
using GDB.Api.Domain.Models;
using GDB.Api.Infrastructure.Repositories.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Services.Implementations
{
    public class WithdrawTransactionCommand
        : ITransactionCommand<WithdrawResponseDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        private readonly ILogger<WithdrawTransactionCommand> _logger;

        public WithdrawTransactionCommand(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ILogger<WithdrawTransactionCommand> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<WithdrawResponseDto> ExecuteAsync(
            TransactionDto transactionDto)
        {
            IAccount account =
                await _accountRepository.GetAccountAsync(
                    transactionDto.AccountNumber);

            if (account == null)
            {
                _logger.LogWarning(
                    "Withdraw failed: account {AccountNumber} not found",
                    transactionDto.AccountNumber);

                throw new AccountException(
                    "Account not found");
            }

            // Domain handles PIN,
            // amount validation,
            // balance validation,
            // account-specific withdrawal rules.
            account.Withdraw(
                transactionDto.Amount,
                transactionDto.Pin);

            // Update balance
            await _accountRepository.UpdateBalanceAsync(
                transactionDto.AccountNumber,
                account.Balance);

            // Save transaction
            await _transactionRepository.SaveTransactionAsync(
                transactionDto.AccountNumber,
                null,
                TransactionType.Withdraw,
                transactionDto.Amount,
                TransactionStatus.Success,
                account.Balance,
                0);

            _logger.LogInformation(
    "Withdrew {Amount} from {AccountNumber}",
    transactionDto.Amount.ToString("C", new CultureInfo("en-IN")),
    transactionDto.AccountNumber);

            return new WithdrawResponseDto
            {
                Balance = account.Balance,
                TransactionStat = TransactionStatus.Success
            };
        }
    }
}
