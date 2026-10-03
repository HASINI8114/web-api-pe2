using System;
using System.Data;

namespace GDB.Api.Data
{
    public interface IGDBInMemoryDB
    {
        DataSet CreateDataSet();
    }
}
