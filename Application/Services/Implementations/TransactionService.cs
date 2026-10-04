using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Application.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionCommandFactory _transactionCommandFactory;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionCommandFactory transactionCommandFactory, ILogger<TransactionService> logger)
        {
            _transactionCommandFactory = transactionCommandFactory;
            _logger = logger;
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
    }
}
