using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IQueuePollerFactory
    {
        IQueuePoller Create(string queueName, CancellationToken cancellationToken);
        void Destroy(IQueuePoller poller);
    }
}
