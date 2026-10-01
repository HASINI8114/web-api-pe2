using System.Data;
using GDB.Api.Data;

namespace GDB.Api.Infrastructure.Repositories
{
    public static class GDBInMemoryDataStore
    {
        public static DataSet DataSet { get; } =
            GDBInMemoryDB.CreateDataSet();
    }
}