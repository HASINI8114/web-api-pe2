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
    public class TransferTransactionCommand
    : ITransactionCommand<TranferFundsResponseDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        private readonly ILogger<TransferTransactionCommand> _logger;

        public TransferTransactionCommand(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ILogger<TransferTransactionCommand> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<TranferFundsResponseDto> ExecuteAsync(
            TransactionDto transactionDto)
        {
            // Get sender
            IAccount fromAccount =
                await _accountRepository.GetAccountAsync(
                    transactionDto.FromAccount);

            if (fromAccount == null)
            {
                _logger.LogWarning(
                    "Transfer failed: from account {AccountNumber} not found",
                    transactionDto.FromAccount);

                throw new AccountException(
                    "From account not found");
            }

            // Get receiver
            IAccount toAccount =
                await _accountRepository.GetAccountAsync(
                    transactionDto.ToAccount);

            if (toAccount == null)
            {
                _logger.LogWarning(
                    "Transfer failed: to account {AccountNumber} not found",
                    transactionDto.ToAccount);

                throw new AccountException(
                    "To account not found");
            }

            // Check sender is active
            if (!fromAccount.CheckIfAccountIsActive())
            {
                throw new InactiveAccountException("From account is inactive");
            }

            // Check receiver is active
            if (!toAccount.CheckIfAccountIsActive())
            {
                throw new InactiveAccountException("To account is inactive");
            }

            // Check PIN
            if (!fromAccount.ValidatePin(
                    transactionDto.Pin))
            {
                throw new InvalidPinException("Invalid PIN.");
            }

            // Withdraw from sender
            fromAccount.Withdraw(
                transactionDto.Amount,
                transactionDto.Pin);

            // Deposit into receiver
            toAccount.Deposit(
                transactionDto.Amount);

            // Save both accounts
            await _accountRepository.SaveAccountsAsync(
                fromAccount,
                toAccount);

            // Save transaction
            await _transactionRepository.SaveTransactionAsync(
                transactionDto.FromAccount,
                transactionDto.ToAccount,
                TransactionType.Transfer,
                transactionDto.Amount,
                TransactionStatus.Success,
                fromAccount.Balance,
                toAccount.Balance);

            _logger.LogInformation(
    "Transferred {Amount} from {FromAccount} to {ToAccount}",
    transactionDto.Amount.ToString("C", new CultureInfo("en-IN")),
    transactionDto.FromAccount,
    transactionDto.ToAccount);

            return new TranferFundsResponseDto
            {
                FromAccountNumber =
                    transactionDto.FromAccount,

                ToAccountNumber =
                    transactionDto.ToAccount,

                Amount =
                    transactionDto.Amount,

                FromAccountBalance =
                    fromAccount.Balance,

                ToAccountBalance =
                    toAccount.Balance,

                TransactionStatus =
                    TransactionStatus.Success
            };
        }
    }
}
