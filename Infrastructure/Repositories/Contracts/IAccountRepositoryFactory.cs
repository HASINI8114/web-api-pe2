namespace GDB.Api.Infrastructure.Repositories.Contracts
{
    public interface IAccountRepositoryFactory
    {
        IAccountRepository Create(string choice);
    }
}
