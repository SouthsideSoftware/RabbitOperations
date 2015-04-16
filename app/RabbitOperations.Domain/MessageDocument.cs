using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Domain
{
    public class MessageDocument
    {
        public MessageDocument()
        {
            Headers = new Dictionary<string, string>();
            MessageTypes = new List<TypeName>();
            BusTechnology = "NServiceBus";
            BusinessKeys = new Dictionary<string, string>();
            EnvironmentId = "Default";
        }

        public long Id { get; set; }

        public string EnvironmentId { get; set; }
        public IDictionary<string, string> BusinessKeys { get; set; }
        public string ContentType { get; set; }
        public string BusTechnology { get; set; }
        public bool IsError { get; set; }

        public AdditionalErrorStatus AdditionalErrorStatus { get; set; }


        public DateTime TimeSent { get; set; }

        public TimeSpan TotalTime { get; set; }

        public TimeSpan ProcessingTime { get; set; }

        public IList<TypeName> MessageTypes { get; set; }

        public IList<string> Keywords { get; set; }

        public SagaInfo SagaInfo { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string Body { get; set; }

        public string Any { get; set; }

        public IList<MessageDocument> Retries { get; set; }

        public bool CanRetry
        {
            get
            {
                return IsError && AdditionalErrorStatus != AdditionalErrorStatus.Resolved &&
                       AdditionalErrorStatus != AdditionalErrorStatus.Closed &&
                       AdditionalErrorStatus != AdditionalErrorStatus.RetryPending;
            }
        }

    }
}
