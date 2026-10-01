using System;
using GDB.Api.Domain.Exceptions;


namespace GDB.Api.Domain.Exceptions
{
    /// <summary>
    /// Purpose: Thrown when an operation is attempted on an inactive account.
    /// </summary>
    public class InactiveAccountException : AccountException
    {
        public InactiveAccountException(string message = "") : base(message) { }
    }
}
