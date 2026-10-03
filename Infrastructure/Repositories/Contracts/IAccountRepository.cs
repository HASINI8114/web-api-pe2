using GDB.Api.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Infrastructure.Repositories.Contracts
{
    public interface IAccountRepository
    {
        Task<IAccount> GetAccountAsync(string accountNumber);
        Task CloseAccountAsync(string accountNumber);
        Task SaveAccountAsync(IAccount account, string pin);
        Task<List<IAccount>> GetAllAccountsAsync();
        Task SaveAccountsAsync(IAccount fromAccount, IAccount toAccount);
        //Task ChangePinAsync(string accountNumber, string oldPin, string newPin);
        Task UpdateBalanceAsync(string accountNumber, decimal balance);

    }
}
