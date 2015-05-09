using System;
using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitOperations.Collector.Service.Interfaces;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class RabbitConnectionFactory : IRabbitConnectionFactory
    {
        protected internal ConcurrentDictionary<string, IConnectionFactory> connectionFactories;

        public RabbitConnectionFactory()
        {
            connectionFactories = new ConcurrentDictionary<string, IConnectionFactory>();
        }

        public IConnectionFactory Create(string connectionString, ushort heartbeatIntervalSeconds = 10)
        {
            Verify.RequireStringNotNullOrWhitespace(connectionString, "connectionString");

            return connectionFactories.GetOrAdd(connectionString, x => new ConnectionFactory
            {
                Uri = connectionString,
                RequestedHeartbeat = heartbeatIntervalSeconds
            });
        }
    }
}