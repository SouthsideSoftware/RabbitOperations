using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;

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
            ApplicationId = "Default";
            Retries = new List<MessageDocument>();
            Body = "";
        }

		public long DocId { get { return Id; } }

        public long Id { get; set; }
        public string ApplicationId { get; set; }
        public IDictionary<string, string> BusinessKeys { get; set; }
        public string ContentType { get; set; }
        public string BusTechnology { get; set; }
        public bool IsError { get; set; }

        public AdditionalErrorStatus AdditionalErrorStatus { get; set; }

        //This property only exists to support serializing the status to a string for Nancy
        public string AdditionalErrorStatusString
        {
            get { return AdditionalErrorStatus.ToString(); }
        }

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
                return IsError && AdditionalErrorStatus != AdditionalErrorStatus.IsRetry && AdditionalErrorStatus != AdditionalErrorStatus.Resolved &&
                       AdditionalErrorStatus != AdditionalErrorStatus.Closed &&
                       AdditionalErrorStatus != AdditionalErrorStatus.RetryPending;
            }
        }

    }
}
