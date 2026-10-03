using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDB.Api.Domain.Enums;

namespace GDB.Api.Application.Dtos
{
    public class TranferFundsResponseDto
    {
        public string FromAccountNumber { get; set; }

        public string ToAccountNumber { get; set; }

        public decimal Amount { get; set; }

        public decimal FromAccountBalance { get; set; }

        public decimal ToAccountBalance { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

    }
}
