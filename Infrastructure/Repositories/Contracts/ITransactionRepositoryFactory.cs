namespace GDB.Api.Infrastructure.Repositories.Contracts
{
    public interface ITransactionRepositoryFactory
    {
        public ITransactionRepository Create();
    }
}
