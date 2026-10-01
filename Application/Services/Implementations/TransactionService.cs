using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Domain.Enums;
using gdb.Logging;
using Microsoft.Extensions.Logging;

namespace GDB.Api.Application.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private static readonly ILogger _logger =
            AppLogger.CreateLogger<TransactionService>();

        public async Task<TResponse> ProcessTransactionAsync<TResponse>(
            TransactionDto transactionDto,
            TransactionType transactionType)
        {
            _logger.LogInformation(
                "Processing transaction {TransactionType}",
                transactionType);

            ITransactionCommand<TResponse> command =
                TransactionCommandFactory.Create<TResponse>(transactionType);

            TResponse response =
                await command.ExecuteAsync(transactionDto);

            _logger.LogInformation(
                "Transaction {TransactionType} completed successfully",
                transactionType);

            return response;
        }
    }
}
