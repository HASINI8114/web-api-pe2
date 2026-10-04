using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using GDB.Api.Domain.Enums;
using GDB.Api.Infrastructure.Repositories;
using GDB.Api.Infrastructure.Repositories.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Services
{
    public class TransactionCommandFactory : ITransactionCommandFactory
    {

        private readonly IAccountRepositoryFactory _accountRepositoryFactory;

        private readonly ITransactionRepositoryFactory _transactionRepositoryFactory;

        private readonly ILoggerFactory _loggerFactory;

        public TransactionCommandFactory(IAccountRepositoryFactory accountRepositoryFactory, ITransactionRepositoryFactory transactionRepositoryFactory, ILoggerFactory loggerFactory)
        {
            _accountRepositoryFactory = accountRepositoryFactory;
            _transactionRepositoryFactory = transactionRepositoryFactory;
            _loggerFactory = loggerFactory;
        }
        public  ITransactionCommand<TResponse> Create<TResponse>(
            TransactionType transactionType)
        {
            IAccountRepository accountRepository =
                _accountRepositoryFactory.Create("DB");

            ITransactionRepository transactionRepository =
                _transactionRepositoryFactory.Create("DB");

            return transactionType switch
            {
                TransactionType.Deposit =>
                    (ITransactionCommand<TResponse>)new DepositTransactionCommand(
                        accountRepository,
                        transactionRepository,
                        _loggerFactory.CreateLogger<DepositTransactionCommand>()),

                TransactionType.Withdraw =>
                    (ITransactionCommand<TResponse>)new WithdrawTransactionCommand(
                        accountRepository,
                        transactionRepository,
                        _loggerFactory.CreateLogger<WithdrawTransactionCommand>()),

                TransactionType.Transfer =>
                    (ITransactionCommand<TResponse>)new TransferTransactionCommand(
                        accountRepository,
                        transactionRepository,
                        _loggerFactory.CreateLogger<TransferTransactionCommand>()),

                _ => throw new ArgumentException(
                    $"Invalid transaction type: {transactionType}")
            };
        }
    }
}
