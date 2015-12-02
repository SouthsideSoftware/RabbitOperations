using System;
using System.Diagnostics.CodeAnalysis;

namespace SouthsideUtility.Core.DesignByContract.Exceptions
{
    /// <summary>
    /// Exception raised when a contract is broken.
    /// Catch this exception type if you wish to differentiate between 
    /// any DesignByContract exception and other runtime exceptions.
    ///  
    /// </summary>
    [ExcludeFromCodeCoverage] //just a simple exception.  No tests needed.
    public class DesignByContractException : Exception
    {
        protected DesignByContractException()
        {
        }

        protected DesignByContractException(string message) : base(message)
        {
        }

        protected DesignByContractException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
