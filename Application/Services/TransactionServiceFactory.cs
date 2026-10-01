using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB.Api.Application.Services
{
    public static class TransactionServiceFactory
    {

        public static ITransactionService Create()
        {
            return new TransactionService();
        }
    }
}
