using System;
using GDB.Api.Domain.Exceptions;


namespace GDB.Api.Domain.Exceptions
{
    /// <summary>
    /// Purpose: Thrown when entered PIN does not match account PIN.
    /// </summary>
    public class InvalidPinException : AccountException
    {
        public InvalidPinException(string message = "") : base(message) { }
    }
}
