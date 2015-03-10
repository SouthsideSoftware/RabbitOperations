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

            QueueName = queueName;
            EnvironmentId = environment.EnvironmentId;
            EnvironmentName = environment.EnvironmentName;
            RabbitConnectionString = environment.RabbitConnectionString;
            MaxMessagesPerRun = environment.MaxMessagesPerRun;
            PollingTimeoutMilliseconds = environment.PollingTimeoutMilliseconds;
            HeartbeatIntervalSeconds = environment.HeartbeatIntervalSeconds;
            IsErrorQueue = environment.ErrorQueue == QueueName;
            DocumentExpirationInHours = environment.DocumentExpirationInHours;
        }

        public bool IsErrorQueue { get; private set; }
        public string EnvironmentId { get; private set; }

        public string EnvironmentName { get; private set; }

        public string QueueName { get; private set; }

        public int MaxMessagesPerRun { get; private set; }

        public int PollingTimeoutMilliseconds { get; private set; }

        public string RabbitConnectionString { get; private set; }

        public int HeartbeatIntervalSeconds { get; private set; }

        public int DocumentExpirationInHours { get; private set; }
    }
}