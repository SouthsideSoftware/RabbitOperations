using System;
using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitOperations.Collector.Service.Interfaces;

namespace RabbitOperations.Collector.Service
{
    public class RabbitConnectionFactory : IRabbitConnectionFactory
    {
        protected internal ConcurrentDictionary<string, IConnectionFactory> connectionFactories;

        public RabbitConnectionFactory()
        {
            connectionFactories = new ConcurrentDictionary<string, IConnectionFactory>();
        }
 
        public IConnectionFactory Create(IQueueSettings queueSettings)
        {
            return connectionFactories.GetOrAdd(queueSettings.RabbitConnectionString, x => new ConnectionFactory
            {
                Uri = queueSettings.RabbitConnectionString,
                RequestedHeartbeat = (ushort) queueSettings.HeartbeatIntervalSeconds
            });
        }
    }
}