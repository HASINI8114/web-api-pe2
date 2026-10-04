using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDB.Api.Application.Dtos.Request;
using GDB.Api.Application.Dtos.Response;
using GDB.Api.Domain.Enums;

namespace GDB.Api.Application.Services.Contracts
{
    public interface ITransactionService
    {
        Task<TResponse> ProcessTransactionAsync<TResponse>(
            TransactionDto transactionDto,
            TransactionType transactionType);

        Task<List<ViewRecentTransactionsResponseDto>> GetRecentTransactionsAsync(ViewRecentTransactionsRequestDto requestDto);
    }
}
