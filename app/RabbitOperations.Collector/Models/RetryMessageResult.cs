using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Models
{
    public class RetryMessageResult
    {
        public RetryMessageResult()
        {
            RetryMessageItems = new List<RetryMessageItem>();
        }
        public IList<RetryMessageItem> RetryMessageItems { get; set; } 
		public string UserSuppliedRetryDestination { get; set; }
    }
}
