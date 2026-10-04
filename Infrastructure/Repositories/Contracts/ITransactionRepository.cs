using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDB.Api.Domain.Enums;
using GDB.Api.Application.Dtos.Response;
using GDB.Api.Application.Dtos.Request;

namespace GDB.Api.Infrastructure.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Task<List<ViewRecentTransactionsResponseDto>> GetRecentTransactionsAsync(
            ViewRecentTransactionsRequestDto request);

        Task SaveTransactionAsync(
           string fromAccountNumber,
           string toAccountNumber,
           TransactionType transactionType,
           decimal amount,
           TransactionStatus transactionStatus,
           decimal balanceAfterFrom,
           decimal balanceAfterTo);
    }
}
