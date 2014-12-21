using System;

namespace RabbitOperations.Utility.DesignByContract.Exceptions
{
    public class ElmahFriendlyInvariantException : Exception
    {
        public ElmahFriendlyInvariantException(InvariantException inner) : base(inner.ToLoggableString(), inner)
        {
        }
    }
}