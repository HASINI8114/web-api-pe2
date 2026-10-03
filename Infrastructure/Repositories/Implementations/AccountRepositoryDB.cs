using GDB.Api.Domain.Enums;
using GDB.Api.Domain.Models;
using GDB.Api.Infrastructure.Repositories.Contracts;
using GDB.Api.Infrastructure.Repositories.Queries;
using System.Data.Common;
using gdb.Logging;
using Microsoft.Extensions.Logging;
using System.Data;
//using System.Data.SqlClient;

namespace GDB.Api.Infrastructure.Repositories.Implementations
{
    internal class AccountRepositoryDB : IAccountRepository
    {
        private readonly IDataBaseConnectionManager _connectionManager;

        public AccountRepositoryDB(IDataBaseConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }
        private static readonly ILogger _logger = AppLogger.CreateLogger<AccountRepositoryDB>();

        public async Task<IAccount> GetAccountAsync(string accountNumber)
        {
            try
            {
                using (DbConnection connection = _connectionManager.GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = AccountQueries.GetAccount;
                        command.CommandType = CommandType.StoredProcedure;

                        AddParameter(
                            command,
                            "@AccountNumber",
                            accountNumber);

                        using (DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                return CreateAccount(reader);
                            }
                        }
                    }
                }

                _logger.LogWarning("Account {AccountNumber} not found in DB", accountNumber);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch account {AccountNumber}", accountNumber);
                throw;
            }
        }


        public async Task SaveAccountAsync(
            IAccount account,
            string pin)
        {
            try
            {
                using (DbConnection connection =
                       _connectionManager.GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    DbTransaction transaction =
                        await connection.BeginTransactionAsync().ConfigureAwait(false);

                    try
                    {
                        long accountId;

                        using (DbCommand command =
                               connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = AccountQueries.CreateAccount;

                            command.CommandType =
                                CommandType.StoredProcedure;

                            AddParameter(
                                command,
                                "@AccountNumber",
                                account.AccountNumber);

                            AddParameter(
                                command,
                                "@Name",
                                account.Name);

                            AddParameter(
                                command,
                                "@Age",
                                account.Age);

                            AddParameter(
                                command,
                                "@AccountType",
                                GetAccountTypeCode(
                                    account.AccountType));

                            AddParameter(
                                command,
                                "@Balance",
                                account.Balance);

                            AddParameter(
                                command,
                                "@AccountStatus",
                                GetAccountStatusCode(
                                    account.Status));

                            AddParameter(
                                command,
                                "@AccountPrivilege",
                                GetAccountPrivilegeCode(
                                    account.Privilege));

                            AddParameter(
                                command,
                                "@Pin",
                                pin);

                            accountId =
                                Convert.ToInt64(
                                    await command.ExecuteScalarAsync().ConfigureAwait(false));
                        }

                        await SaveAccountTypeAsync(
                            account,
                            accountId,
                            connection,
                            transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {   
                        _logger.LogWarning(ex, "Rolling back SaveAccountAsync for {AccountNumber}", account.AccountNumber);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save account {AccountNumber}", account.AccountNumber);
                throw;
            }
        }


        private async Task SaveAccountTypeAsync(
            IAccount account,
            long accountId,
            DbConnection connection,
            DbTransaction transaction)
        {
            if (account is SavingsAccount savings)
            {
                await SaveSavingsAccountAsync(
                    savings,
                    accountId,
                    connection,
                    transaction);
            }
            else if (account is CurrentAccount current)
            {
                await SaveCurrentAccountAsync(
                    current,
                    accountId,
                    connection,
                    transaction);
            }
            else if (account is FixedDepositAccount fixedDeposit)
            {
                await SaveFixedDepositAccountAsync(
                    fixedDeposit,
                    accountId,
                    connection,
                    transaction);
            }
            else if (account is SalaryAccount salary)
            {
                await SaveSalaryAccountAsync(
                    salary,
                    accountId,
                    connection,
                    transaction);
            }
            else
            {
                _logger.LogWarning("No subtype table for account {AccountNumber} of type {AccountType}", account.AccountNumber, account.GetType().Name);
            }
        }


        private async Task SaveSavingsAccountAsync(
            SavingsAccount savings,
            long accountId,
            DbConnection connection,
            DbTransaction transaction)
        {
            using (DbCommand command =
                   connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
    AccountQueries.InsertSavingsAccount;

                command.CommandType =
                    CommandType.StoredProcedure;

                AddParameter(
                    command,
                    "@AccountId",
                    accountId);

                AddParameter(
                    command,
                    "@InterestRate",
                    savings.InterestRate / 100.0);

                AddParameter(
                    command,
                    "@MinimumBalance",
                    savings.MinBalance);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }


        private async Task SaveCurrentAccountAsync(
            CurrentAccount current,
            long accountId,
            DbConnection connection,
            DbTransaction transaction)
        {
            using (DbCommand command =
                   connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
     AccountQueries.InsertCurrentAccount;

                command.CommandType =
                    CommandType.StoredProcedure;

                AddParameter(
                    command,
                    "@AccountId",
                    accountId);

                AddParameter(
                    command,
                    "@OverdraftLimit",
                    current.OverdraftLimit);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }


        private async Task SaveFixedDepositAccountAsync (
            FixedDepositAccount fixedDeposit,
            long accountId,
            DbConnection connection,
            DbTransaction transaction)
        {
            using (DbCommand command =
                   connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
    AccountQueries.InsertFixedDepositAccount;

                command.CommandType =
                    CommandType.StoredProcedure;

                AddParameter(
                    command,
                    "@AccountId",
                    accountId);

                AddParameter(
                    command,
                    "@InterestRate",
                    fixedDeposit.InterestRate / 100.0);

                AddParameter(
                    command,
                    "@TenureMonths",
                    fixedDeposit.TenureMonths);

                AddParameter(
                    command,
                    "@PrincipalAmount",
                    fixedDeposit.Balance);

                decimal maturityAmount = fixedDeposit.CalculateMaturityAmount();

                AddParameter(
                    command,
                    "@MaturityAmount",
                    maturityAmount);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        
        private async Task SaveSalaryAccountAsync(
            SalaryAccount salary,
            long accountId,
            DbConnection connection,
            DbTransaction transaction)
        {
            using (DbCommand command =
                   connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
    AccountQueries.InsertSalaryAccount;

                command.CommandType =
                    CommandType.StoredProcedure;
                AddParameter(
                    command,
                    "@AccountId",
                    accountId);

                AddParameter(
                    command,
                    "@EmployerName",
                    salary.EmployerName);

                AddParameter(
                    command,
                    "@InactiveMonths",
                    salary.InactiveMonths);

                AddParameter(
                    command,
                    "@SalaryAmount",
                    salary.Balance);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }


        public async Task UpdateBalanceAsync(
            string accountNumber,
            decimal balance)
        {
            try
            {
                using DbConnection connection =
                    _connectionManager.GetConnection();

                await connection.OpenAsync().ConfigureAwait(false);

                using DbCommand command =
                    connection.CreateCommand();

                command.CommandText =
                    AccountQueries.UpdateBalance;

                AddParameter(
                    command,
                    "@Balance",
                    balance);

                AddParameter(
                    command,
                    "@AccountNumber",
                    accountNumber);

                int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("UpdateBalance affected no rows for account {AccountNumber}", accountNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update balance for account {AccountNumber}", accountNumber);
                throw;
            }
        }


        public async Task CloseAccountAsync(
            string accountNumber)
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
                            AccountQueries.CloseAccount;

                        AddParameter(
                            command,
                            "@AccountNumber",
                            accountNumber);

                        int rowsAffected =
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (rowsAffected == 0)
                        {
                            _logger.LogWarning("CloseAccount: account {AccountNumber} not found", accountNumber);
                            throw new Exception(
                                "Account not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close account {AccountNumber}", accountNumber);
                throw;
            }
        }


        public async Task<List<IAccount>> GetAllAccountsAsync()
        {
            try
            {
                List<IAccount> accounts =
                    new List<IAccount>();

                using (DbConnection connection =
                       _connectionManager.GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (DbCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText =
                            AccountQueries.GetAllAccounts;

                        using (DbDataReader reader =
                               await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                accounts.Add(
                                    CreateAccount(reader));
                            }
                        }
                    }
                }

                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all accounts");
                throw;
            }
        }


        public async Task SaveAccountsAsync(
            IAccount fromAccount,
            IAccount toAccount)
        {
            try
            {
                using (DbConnection connection =
                       _connectionManager.GetConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    DbTransaction transaction =
                        connection.BeginTransaction();

                    try
                    {
                        using (DbCommand command =
                               connection.CreateCommand())
                        {
                            command.Transaction =
                                transaction;

                            command.CommandText =
                                AccountQueries.SaveAccounts;

                            AddParameter(
                                command,
                                "@Balance",
                                fromAccount.Balance);

                            AddParameter(
                                command,
                                "@AccountNumber",
                                fromAccount.AccountNumber);

                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                            command.Parameters[
                                "@Balance"].Value =
                                toAccount.Balance;

                            command.Parameters[
                                "@AccountNumber"].Value =
                                toAccount.AccountNumber;

                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Rolling back SaveAccounts for {FromAccount} -> {ToAccount}", fromAccount.AccountNumber, toAccount.AccountNumber);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save accounts {FromAccount} -> {ToAccount}", fromAccount.AccountNumber, toAccount.AccountNumber);
                throw;
            }
        }


        private IAccount CreateAccount(
            DbDataReader reader)
        {
            string accountNumber =
                reader["AccountNumber"].ToString();

            string name =
                reader["Name"].ToString();

            int age =
                Convert.ToInt32(
                    reader["Age"]);

            decimal balance =
                Convert.ToDecimal(
                    reader["Balance"]);

            AccountType accountType =
                ConvertAccountType(
                    reader["AccountType"].ToString());

            AccountStatus status =
                ConvertAccountStatus(
                    reader["AccountStatus"].ToString());

            AccountPrivilege privilege =
                ConvertAccountPrivilege(
                    reader["AccountPrivilege"].ToString());

            string pin =
                reader["Pin"].ToString();

            switch (accountType)
            {
                case AccountType.Savings:
                    return CreateSavingsAccount(
                        reader,
                        accountNumber,
                        name,
                        age,
                        balance,
                        accountType,
                        status,
                        pin,
                        privilege);

                case AccountType.Current:
                    return CreateCurrentAccount(
                        reader,
                        accountNumber,
                        name,
                        age,
                        balance,
                        accountType,
                        status,
                        pin,
                        privilege);

                case AccountType.FixedDeposit:
                    return CreateFixedDepositAccount(
                        reader,
                        accountNumber,
                        name,
                        age,
                        balance,
                        accountType,
                        status,
                        pin,
                        privilege);

                case AccountType.Salary:
                    return CreateSalaryAccount(
                        reader,
                        accountNumber,
                        name,
                        age,
                        balance,
                        accountType,
                        status,
                        pin,
                        privilege);

                default:
                    throw new Exception(
                        "Unknown account type.");
            }
        }


        private IAccount CreateSavingsAccount(
            DbDataReader reader,
            string accountNumber,
            string name,
            int age,
            decimal balance,
            AccountType accountType,
            AccountStatus status,
            string pin,
            AccountPrivilege privilege)
        {
            decimal minBalance =
                reader["SavingsMinimumBalance"] ==
                DBNull.Value
                ? 1000.0m
                : Convert.ToDecimal(
                    reader["SavingsMinimumBalance"]);

            double interestRate =
                reader["SavingsInterestRate"] ==
                DBNull.Value
                ? 4.0
                : Convert.ToDouble(
                    reader["SavingsInterestRate"]) * 100;

            return new SavingsAccount(
                accountNumber,
                name,
                age,
                balance,
                accountType,
                status,
                pin,
                privilege,
                minBalance,
                interestRate);
        }


        private IAccount CreateCurrentAccount(
            DbDataReader reader,
            string accountNumber,
            string name,
            int age,
            decimal balance,
            AccountType accountType,
            AccountStatus status,
            string pin,
            AccountPrivilege privilege)
        {
            decimal overdraftLimit =
                reader["OverdraftLimit"] ==
                DBNull.Value
                ? 25000.0m
                : Convert.ToDecimal(
                    reader["OverdraftLimit"]);

            return new CurrentAccount(
                accountNumber,
                name,
                age,
                balance,
                accountType,
                status,
                pin,
                privilege,
                overdraftLimit);
        }


        private IAccount CreateFixedDepositAccount(
            DbDataReader reader,
            string accountNumber,
            string name,
            int age,
            decimal balance,
            AccountType accountType,
            AccountStatus status,
            string pin,
            AccountPrivilege privilege)
        {
            int tenureMonths =
                reader["TenureMonths"] ==
                DBNull.Value
                ? 12
                : Convert.ToInt32(
                    reader["TenureMonths"]);

            double interestRate =
                reader["FixedDepositInterestRate"] ==
                DBNull.Value
                ? 6.5
                : Convert.ToDouble(
                    reader["FixedDepositInterestRate"]) * 100;

            return new FixedDepositAccount(
                accountNumber,
                name,
                age,
                balance,
                accountType,
                status,
                pin,
                privilege,
                tenureMonths,
                interestRate);
        }


        private IAccount CreateSalaryAccount(
            DbDataReader reader,
            string accountNumber,
            string name,
            int age,
            decimal balance,
            AccountType accountType,
            AccountStatus status,
            string pin,
            AccountPrivilege privilege)
        {
            string employerName =
                reader["EmployerName"] ==
                DBNull.Value
                ? "TechCorp"
                : reader["EmployerName"].ToString();

            return new SalaryAccount(
                accountNumber,
                name,
                age,
                balance,
                accountType,
                status,
                pin,
                privilege,
                employerName);
        }


        private AccountType ConvertAccountType(
            string value)
        {
            switch (value)
            {
                case "SAVINGS":
                    return AccountType.Savings;

                case "CURRENT":
                    return AccountType.Current;

                case "FIXED_DEPOSIT":
                    return AccountType.FixedDeposit;

                case "SALARY":
                    return AccountType.Salary;

                default:
                    _logger.LogWarning("Unknown account type code {AccountTypeCode} read from DB", value);
                    throw new Exception(
                        "Invalid account type: " + value);
            }
        }


        private AccountStatus ConvertAccountStatus(
            string value)
        {
            return (AccountStatus)Enum.Parse(
                typeof(AccountStatus),
                value,
                true);
        }


        private AccountPrivilege ConvertAccountPrivilege(
            string value)
        {
            return (AccountPrivilege)Enum.Parse(
                typeof(AccountPrivilege),
                value,
                true);
        }


        private string GetAccountTypeCode(
            AccountType accountType)
        {
            switch (accountType)
            {
                case AccountType.Savings:
                    return "SAVINGS";

                case AccountType.Current:
                    return "CURRENT";

                case AccountType.FixedDeposit:
                    return "FIXED_DEPOSIT";

                case AccountType.Salary:
                    return "SALARY";

                default:
                    throw new Exception(
                        "Invalid account type.");
            }
        }


        private string GetAccountStatusCode(
            AccountStatus status)
        {
            return status.ToString().ToUpper();
        }


        private string GetAccountPrivilegeCode(
            AccountPrivilege privilege)
        {
            return privilege.ToString().ToUpper();
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