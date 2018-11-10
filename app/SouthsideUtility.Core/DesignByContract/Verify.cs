using System;
using System.Collections.Generic;
using System.Linq;
using SouthsideUtility.Core.DesignByContract.Exceptions;

namespace SouthsideUtility.Core.DesignByContract
{
    public static class Verify
    {
        /// <summary>
        /// Precondition check
        /// </summary>
        public static void Require(bool assertion, string message = "Precondition failed.", Exception inner = null)
        {
            if (!assertion) throw new PreconditionException(message, inner);
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        public static void Ensure(bool assertion, string message = "Postcondition failed.", Exception inner = null)
        {
            if (!assertion) throw new PostconditionException(message, inner);
        }

        /// <summary>
        /// Require that the given object is not null 
        /// </summary>
        public static void RequireNotNull(object obj, string objName)
        {
            Require(obj != null, string.Format("{0} must not be null.", objName));
        }

        /// <summary>
        /// Require that the given object is not null 
        /// </summary>
        public static void RequireStringNotNullOrWhitespace(string obj, string objName)
        {
            Require(!string.IsNullOrWhiteSpace(obj), string.Format("{0} must not be null, empty or whitespace.", objName));
        }

        /// <summary>
        /// Verifies a set of invariants on a particular domain object.
        /// </summary>
        /// <param name="invariants">List of invariants to check.</param>
        /// <param name="domainObjectType">The type of domain object being validated.</param>
        /// <param name="clientErrorCode">A client-specific error code that provides additional details about the error.</param>
        /// <exception cref="InvariantException">Thrown if any of the invariants fail.  Domain validation error(s) in the exception list
        /// the specific failures.</exception>
        public static void InvariantCheck(IList<Invariant> invariants, Type domainObjectType, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
        {
            InvariantCheck( invariants, domainObjectType.Name, clientErrorCode );
        }

        /// <summary>
        /// Verifies a set of invariants on a particular domain object.
        /// </summary>
        /// <param name="invariants">List of invariants to check.</param>
        /// <param name="domainObjectType">The type of domain object being validated.</param>
        /// <param name="clientErrorCode">A client-specific error code that provides additional details about the error.</param>
        /// <exception cref="InvariantException">Thrown if any of the invariants fail.  Domain validation error(s) in the exception list
        /// the specific failures.</exception>
        public static void InvariantCheck(IList<Invariant> invariants, string domainObjectType, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
        {
            InvariantException exception = null;

            //Check all of the invariants and add DomainValidationErrors if they fail.
            foreach( Invariant invariant in invariants.Where( invariant => !invariant.Condition ) )
            {
                var domainValidationError = new DomainValidationError( invariant.PropertyName, invariant.Message );
                if( exception == null )
                {
                    exception = new InvariantException( domainValidationError, String.Format( "There was a problem validating the {0}.", domainObjectType ), clientErrorCode );
                }
                else
                {
                    exception.Add( domainValidationError );
                }
            }

            if( exception != null )
            {
                throw exception;
            }
        }

        /// <summary>
        /// Verifies a single invariant on a particular domain object.
        /// </summary>
        /// <param name="assertion">The condition to check.  If true, nothing happens.  If false, an exception is thrown.</param>
        /// <param name="propertyName">The name of the property being checked.</param>
        /// <param name="message">A message describing the failed invariant check (assuming it failed).</param>
        /// <param name="domainObjectType">The type of domain object being validated.</param>
        /// <exception cref="InvariantException">Thrown if any of the invariants fail.  Domain validation error(s) in the exception list
        /// the specific failures.</exception>
        public static void InvariantCheck(bool assertion, string propertyName, string message, Type domainObjectType)
        {
            if (!assertion) throw new InvariantException(new DomainValidationError(propertyName, message), String.Format("There was a problem validating the {0}. {1}.", domainObjectType.Name, message));
        }

        public static void InvariantCheck(bool assertion, string message, ClientErrorCode clientErrorCode = ClientErrorCode.NotSpecified)
        {
            if(!assertion) throw new InvariantException(new DomainValidationError(message), "Invariant exception occurred. See errors collection for additional details.", clientErrorCode);
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        public static void Assert(bool assertion, string message = "Assertion failed.", Exception inner = null)
        {
            if (!assertion) throw new AssertionException(message, inner);
        }
    }
}