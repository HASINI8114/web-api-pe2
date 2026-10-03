using System.Data.Common;

namespace GDB.Api.Infrastructure.Repositories.Contracts
{
    public interface IDataBaseConnectionManager
    {
        DbConnection GetConnection();
    }
}
