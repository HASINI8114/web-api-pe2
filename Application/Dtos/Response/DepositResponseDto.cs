using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDB.Api.Domain.Enums;

namespace GDB.Api.Application.Dtos.Response
{
    public class DepositResponseDto
    {
        public decimal Balance { get; set; }

        public TransactionStatus TransactionStat { get; set; }
    }
}
