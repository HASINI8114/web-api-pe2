using GDB.Api.Domain.Enums;
using GDB.Api.Domain.Models;

namespace GDB.Api.Domain
{
    public interface IAccountFactory
    {
        Account CreateAccount(AccountType accountType,
                                        string accountNumber,
                                        string name,
                                        int age,
                                        decimal balance,
                                        AccountStatus status,
                                        string pin,
                                        AccountPrivilege privilege,
                                        decimal overdraftLimit = 25000.0m,
                                        int tenureMonths = 12,
                                        double interestRate = 6.5,
                                        decimal minBalance = 1000.0m,
                                        string employerName = "TechCorp");
    }
}
