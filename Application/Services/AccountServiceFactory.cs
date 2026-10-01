using GDB.Api.Application.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using GDB.Api.Application.Services.Contracts;

namespace GDB.Api.Application.Services
{
    public class AccountServiceFactory
    {
        public static IAccountService Create()
        {
            return new AccountService();
        }
    }
}