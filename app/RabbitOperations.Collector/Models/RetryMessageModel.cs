using System.Collections.Generic;
using RabbitMQ.Client.Apigen.Attributes;

namespace RabbitOperations.Collector.Models
{
    public class RetryMessageModel
    {
        public string UserSuppliedRetryDestination { get; set; }
        public RetryMessageModel()
        {
            RetryIds = new List<long>();
        }

        public IList<long> RetryIds { get; set; } 
		public bool ForceRetry { get; set; }
    }
}