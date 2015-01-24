using System;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace RabbitOperations.Collector.Service.Interfaces
{
    public class RabbitConnectionFactory : IRabbitConnectionFactory
    {
        public ConcurrentDictionary<string, IConnectionFactory> ConnectionFactories;

        public RabbitConnectionFactory()
        {
            ConnectionFactories = new ConcurrentDictionary<string, IConnectionFactory>();
        }
 
        public IConnectionFactory Create(IQueueSettings queueSettings)
        {
            return ConnectionFactories.GetOrAdd(queueSettings.RabbitConnectionString, x => new ConnectionFactory
            {
                Uri = queueSettings.RabbitConnectionString,
                RequestedHeartbeat = (ushort) queueSettings.HeartbeatIntervalSeconds
            });
        }
    }
}