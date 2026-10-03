using GDB.Api.Domain.Enums;

namespace GDB.Api.Application.Services.Contracts
{
    public interface ITransactionCommandFactory
    {
        ITransactionCommand<TResponse> Create<TResponse>(TransactionType transactionType);
    }
}
