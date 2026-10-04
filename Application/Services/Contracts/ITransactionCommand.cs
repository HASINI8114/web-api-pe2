using GDB.Api.Application.Dtos.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Services.Contracts
{
    public interface ITransactionCommand<TResponse>
    {
        Task<TResponse> ExecuteAsync(
            TransactionDto transactionDto);
    }
}
