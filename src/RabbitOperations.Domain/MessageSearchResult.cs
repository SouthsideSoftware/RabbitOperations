using System;
using System.Collections.Generic;

namespace RabbitOperations.Domain
{
    public class MessageSearchResult
    {
        public MessageSearchResult()
        {
            MessageTypes = new List<TypeName>();
        }

        public long Id { get; set; }
        public string ApplicationId { get; set; }
        public bool IsError { get; set; }

        public AdditionalErrorStatus AdditionalErrorStatus { get; set; }

        //This property only exists to support serializing the status to a string for Nancy
        public string AdditionalErrorStatusString
        {
            get { return AdditionalErrorStatus.ToString(); }
        }

        public DateTime TimeSent { get; set; }

        public IList<TypeName> MessageTypes { get; set; }

        public string Any { get; set; }

        public bool CanRetry
        {
            get
            {
                return IsError && AdditionalErrorStatus != AdditionalErrorStatus.IsRetry && AdditionalErrorStatus != AdditionalErrorStatus.Resolved &&
                       AdditionalErrorStatus != AdditionalErrorStatus.Closed &&
                       AdditionalErrorStatus != AdditionalErrorStatus.RetryPending;
            }
        }

    }
}