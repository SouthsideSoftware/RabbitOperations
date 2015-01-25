using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitOperations.Collector.Host.Interfaces
{
    public interface ISubHostFactory
    {
        IQueuePollerHost CreatePollerHost();
        IWebHost CreateWebHost();
    }
}
