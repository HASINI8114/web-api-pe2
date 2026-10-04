using GDB.Api.Application.Services.Contracts;
using GDB.Api.Domain.Enums;
using Microsoft.Extensions.Logging;
using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Application.Dtos.Request;
using GDB.Api.Application.Dtos.Response;

namespace GDB.Api.Application.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionCommandFactory _transactionCommandFactory;
        private readonly ITransactionRepository _transactionRepository; 
        private static readonly ILogger _logger =
            AppLogger.CreateLogger<TransactionService>();

        public TransactionService(ITransactionCommandFactory transactionCommandFactory, ITransactionRepositoryFactory transactionRepositoryfactory)
        {
            _transactionCommandFactory = transactionCommandFactory;
            _transactionRepository = transactionRepositoryfactory.Create(); 
        }

        public async Task<TResponse> ProcessTransactionAsync<TResponse>(
            TransactionDto transactionDto,
            TransactionType transactionType)
        {
            _logger.LogInformation(
                "Processing transaction {TransactionType}",
                transactionType);

            ITransactionCommand<TResponse> command =
                _transactionCommandFactory.Create<TResponse>(transactionType);

            TResponse response =
                await command.ExecuteAsync(transactionDto);

            _logger.LogInformation(
                "Transaction {TransactionType} completed successfully",
                transactionType);

            return response;
        }

        public async Task<List<ViewRecentTransactionsResponseDto>> GetRecentTransactionsAsync(
            ViewRecentTransactionsRequestDto requestDto)
        {
            _logger.LogInformation(
                "Retrieving recent transactions for account {AccountNumber}",
                requestDto.AccountNumber);
            var transactions = await _transactionRepository.GetRecentTransactionsAsync(requestDto);
            _logger.LogInformation(
                "Retrieved {TransactionCount} recent transactions for account {AccountNumber}",
                transactions.Count,
                requestDto.AccountNumber);
            return transactions;
        }   
    }
}
