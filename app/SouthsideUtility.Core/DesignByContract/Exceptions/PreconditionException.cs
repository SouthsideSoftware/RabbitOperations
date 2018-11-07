using System;
using System.Diagnostics.CodeAnalysis;

namespace SouthsideUtility.Core.DesignByContract.Exceptions
{
    /// <summary>
    /// Exception raised when a precondition fails.
    /// </summary>
    [ExcludeFromCodeCoverage] //just a simple exception.  No tests needed.
    public class PreconditionException : DesignByContractException
    {
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException() { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message) : base(message) { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message, Exception inner) : base(message, inner) { }
    }
}
