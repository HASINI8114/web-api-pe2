using System;
using GDB.Api.Domain.Exceptions;


namespace GDB.Api.Domain.Exceptions
{
    /// <summary>
    /// Purpose: Thrown when withdrawal breaches minimum balance requirement.
    /// </summary>
    public class MinimumBalanceViolationException : AccountException
    {
        public MinimumBalanceViolationException(string message = "") : base(message) { }
    }
}
