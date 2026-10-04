using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using GDB.Api.Domain.Enums;
using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Infrastructure.Repositories.Queries;
using System.Data.Common;
using GDB.Api.Application.Dtos.Response;
using GDB.Api.Application.Dtos.Request;

namespace GDB.Api.Infrastructure.Repositories.Implementations
{
    public class TransactionRepositoryDB : ITransactionRepository
    {
        private readonly IDataBaseConnectionManager _connectionManager;
        private readonly ILogger<TransactionRepositoryDB> _logger;

        public TransactionRepositoryDB(IDataBaseConnectionManager connectionManager, ILogger<TransactionRepositoryDB> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }
        public async Task<List<ViewRecentTransactionsResponseDto>> GetRecentTransactionsAsync(
            ViewRecentTransactionsRequestDto requestDto)
        {
            try
            {
                List<ViewRecentTransactionsResponseDto> transactions =
                    new List<ViewRecentTransactionsResponseDto>();

                using (DbConnection connection =
                       _connectionManager.GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (DbCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText =
                            TransactionQueries.GetRecentTransactions;

                        int offset = (requestDto.PageNumber - 1) * requestDto.PageSize;

                        AddParameter(command, "@AccountNumber", requestDto.AccountNumber);

                        AddParameter(command, "@Offset", offset);

                        AddParameter(command, "@PageSize", requestDto.PageSize);

                        using (DbDataReader reader =
                               await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                ViewRecentTransactionsResponseDto transaction =
                                    new ViewRecentTransactionsResponseDto();

                                transaction.TransactionId =
                                    Convert.ToInt32(
                                        reader["TransactionId"]);

                                transaction.FromAccountNumber =
                                    reader["FromAccountNumber"] == DBNull.Value
                                        ? null
                                        : reader["FromAccountNumber"].ToString();

                                transaction.ToAccountNumber =
                                    reader["ToAccountNumber"] == DBNull.Value
                                        ? null
                                        : reader["ToAccountNumber"].ToString();

                                transaction.Amount =
                                    Convert.ToDecimal(
                                        reader["Amount"]);

                                transaction.TransactionType =
                                    (TransactionType)Enum.Parse(
                                        typeof(TransactionType),
                                        reader["TransactionType"].ToString(),
                                        true);

                                transaction.TransactionStatus =
                                    (TransactionStatus)Enum.Parse(
                                        typeof(TransactionStatus),
                                        reader["TransactionStatus"].ToString(),
                                        true);

                                transaction.Timestamp =
                                    Convert.ToDateTime(
                                        reader["Timestamp"]);

                                transaction.BalanceAfterFrom =
                                    reader["BalanceAfterFrom"] == DBNull.Value
                                        ? null
                                        : Convert.ToDecimal(
                                            reader["BalanceAfterFrom"]);

                                transaction.BalanceAfterTo =
                                    reader["BalanceAfterTo"] == DBNull.Value
                                        ? null
                                        : Convert.ToDecimal(
                                            reader["BalanceAfterTo"]);

                                transactions.Add(transaction);
                            }
                        }
                    }
                }

                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch recent transactions for account {AccountNumber}", requestDto.AccountNumber);
                throw;
            }
        }


        public async Task SaveTransactionAsync(
            string fromAccountNumber,
            string toAccountNumber,
            TransactionType transactionType,
            decimal amount,
            TransactionStatus transactionStatus,
            decimal balanceAfterFrom,
            decimal balanceAfterTo)
        {
            try
            {
                using (DbConnection connection =
                       _connectionManager.GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (DbCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText =
                            TransactionQueries.InsertTransaction;

                        AddParameter(
                            command,
                            "@TransactionType",
                            transactionType.ToString().ToUpper());

                        AddParameter(
                            command,
                            "@FromAccountNumber",
                            string.IsNullOrEmpty(fromAccountNumber)
                                ? (object)DBNull.Value
                                : fromAccountNumber);

                        AddParameter(
                            command,
                            "@ToAccountNumber",
                            string.IsNullOrEmpty(toAccountNumber)
                                ? (object)DBNull.Value
                                : toAccountNumber);

                        AddParameter(
                            command,
                            "@Amount",
                            amount);

                        AddParameter(
                            command,
                            "@TransactionStatus",
                            transactionStatus.ToString().ToUpper());

                        AddParameter(
                            command,
                            "@BalanceAfterFrom",
                            balanceAfterFrom);

                        AddParameter(
                            command,
                            "@BalanceAfterTo",
                            balanceAfterTo);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                // Balances are already persisted by the caller; this record is needed to reconcile manually
                _logger.LogError(
                    ex,
                    "Failed to save {TransactionType} transaction of {Amount} from {FromAccount} to {ToAccount}",
                    transactionType,
                    amount,
                    fromAccountNumber,
                    toAccountNumber);
                throw;
            }
        }


        private void AddParameter(
            DbCommand command,
            string parameterName,
            object value)
        {
            DbParameter parameter =
                command.CreateParameter();

            parameter.ParameterName =
                parameterName;

            parameter.Value =
                value ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }
    }
}