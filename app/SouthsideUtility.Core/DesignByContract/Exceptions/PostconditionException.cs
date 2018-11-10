using System;
using System.Diagnostics.CodeAnalysis;

namespace SouthsideUtility.Core.DesignByContract.Exceptions
{
    /// <summary>
    /// Exception raised when a postcondition fails.
    /// </summary>
    [ExcludeFromCodeCoverage] //just a simple exception.  No tests needed.
    public class PostconditionException : DesignByContractException
    {
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException() { }
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message) : base(message) { }
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message, Exception inner) : base(message, inner) { }
    }
}
