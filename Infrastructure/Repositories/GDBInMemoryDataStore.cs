using GDB.Api.Data;
using GDB.Api.Infrastructure.Repositories.Contracts;
using System.Data;

namespace GDB.Api.Infrastructure.Repositories
{
    public class GDBInMemoryDataStore: IGDBInMemoryDataStore
    {
        public DataSet DataSet { get; }

        public GDBInMemoryDataStore(IGDBInMemoryDB gDBInMemoryDB)
        {
            DataSet = gDBInMemoryDB.CreateDataSet();
        }
        
    }
}