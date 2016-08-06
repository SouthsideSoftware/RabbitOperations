using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitOperations.Domain.Configuration;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public interface IApplicationListenerFactory
    {
        IApplicationListener Create(IApplicationConfiguration applicationConfiguration, CancellationToken cancellationToken);
    }
}
