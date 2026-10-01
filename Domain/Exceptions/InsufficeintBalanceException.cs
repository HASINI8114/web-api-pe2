using System;
using GDB.Api.Domain.Exceptions;

namespace GDB.Api.Domain.Exceptions
{
    /// <summary>
    /// Purpose: Thrown when withdrawal amount exceeds available funds.
    /// </summary>
    public class InsufficientBalanceException : AccountException
    {
        public InsufficientBalanceException(string message = "") : base(message) { }
    }
}
