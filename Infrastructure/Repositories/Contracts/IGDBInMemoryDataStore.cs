using System;
using System.Data;

namespace GDB.Api.Infrastructure.Repositories.Contracts
{
    public interface IGDBInMemoryDataStore
    {
        DataSet DataSet { get; }
    }
}
