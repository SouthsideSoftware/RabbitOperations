using System.Collections.Generic;

namespace RabbitOperations.Collector.Models
{
    public class RetryMessageModel
    {
        public RetryMessageModel()
        {
            RetryIds = new List<long>();
        }

        public IList<long> RetryIds { get; set; } 
    }
}