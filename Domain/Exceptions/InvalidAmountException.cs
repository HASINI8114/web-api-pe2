using System;
using GDB.Api.Domain.Exceptions;


namespace GDB.Api.Domain.Exceptions
{
    /// <summary>
    /// Purpose: Thrown when deposit or withdrawal amount <= 0.
    /// </summary>
    public class InvalidAmountException : AccountException
    {
        public InvalidAmountException(string message = "") : base(message) { }
    }
}
