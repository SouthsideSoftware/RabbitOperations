using System;

namespace SouthsideUtility.Core.DesignByContract.Exceptions
{
    public class ElmahFriendlyInvariantException : Exception
    {
        public ElmahFriendlyInvariantException(InvariantException inner) : base(inner.ToLoggableString(), inner)
        {
        }
    }
}