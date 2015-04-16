using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Models
{
    public class RetryMessageResult
    {
        public bool IsSuccess { get; set; }
        public long Retryid { get; set; }
    }
}
