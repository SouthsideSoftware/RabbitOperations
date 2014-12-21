using System;
using System.Diagnostics.CodeAnalysis;

namespace RabbitOperations.Utility.DesignByContract.Exceptions
{
    /// <summary>
    /// Exception raised when an assertion fails.
    /// </summary>
    [ExcludeFromCodeCoverage] //just a simple exception.  No tests needed.
    public class AssertionException : DesignByContractException
    {
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException() { }
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message) : base(message) { }
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message, Exception inner) : base(message, inner) { }
    }
}
