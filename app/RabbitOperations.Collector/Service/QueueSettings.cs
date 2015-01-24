using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class QueueSettings : IQueueSettings
    {
        public QueueSettings(string queueName, EnvironmentConfiguration environment)
        {
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireNotNull(environment, "environment");
            
        }

        public string EnvironmentId { get; private set; }

        public string EnvironmentName { get; private set; }

        public string QueueName { get; private set; }

        public int MaxMessagesPerRun { get; private set; }

        public int PollingTimeout { get; private set; }

        public string RabbitConnectionString { get; private set; }

        public int HeartbeatIntervalSeconds { get; private set; }
    }
}