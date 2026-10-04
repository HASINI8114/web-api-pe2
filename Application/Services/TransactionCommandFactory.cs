using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using GDB.Api.Domain.Enums;
using GDB.Api.Infrastructure.Repositories;
using GDB.Api.Infrastructure.Repositories.Contracts;
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

        public TransactionCommandFactory(IAccountRepositoryFactory accountRepositoryFactory, ITransactionRepositoryFactory transactionRepositoryFactory)
        {
            _accountRepositoryFactory = accountRepositoryFactory;
            _transactionRepositoryFactory = transactionRepositoryFactory;
        }
        public  ITransactionCommand<TResponse> Create<TResponse>(
            TransactionType transactionType)
        {
            IAccountRepository accountRepository =
                _accountRepositoryFactory.Create();

            ITransactionRepository transactionRepository =
                _transactionRepositoryFactory.Create();

            return transactionType switch
            {
                TransactionType.Deposit =>
                    (ITransactionCommand<TResponse>)new DepositTransactionCommand(
                        accountRepository,
                        transactionRepository),

                TransactionType.Withdraw =>
                    (ITransactionCommand<TResponse>)new WithdrawTransactionCommand(
                        accountRepository,
                        transactionRepository),

                TransactionType.Transfer =>
                    (ITransactionCommand<TResponse>)new TransferTransactionCommand(
                        accountRepository,
                        transactionRepository),

                _ => throw new ArgumentException(
                    $"Invalid transaction type: {transactionType}")
            };
        }
    }
}
