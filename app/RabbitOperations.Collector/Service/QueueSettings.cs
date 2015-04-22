using System;
using RabbitOperations.Collector.Service.Interfaces;
using RabbitOperations.Domain.Configuration;
using SouthsideUtility.Core.DesignByContract;

namespace RabbitOperations.Collector.Service
{
    public class QueueSettings : IQueueSettings
    {
        public QueueSettings(string queueName, ApplicationConfiguration application)
        {
            Verify.RequireStringNotNullOrWhitespace(queueName, "queueName");
            Verify.RequireNotNull(application, "application");

            QueueName = queueName;
            ApplicationId = application.ApplicationId;
            ApplicationName = application.ApplicationName;
            RabbitConnectionString = application.RabbitConnectionString;
            MaxMessagesPerRun = application.MaxMessagesPerRun;
            PollingTimeoutMilliseconds = application.PollingTimeoutMilliseconds;
            HeartbeatIntervalSeconds = application.HeartbeatIntervalSeconds;
            IsErrorQueue = application.ErrorQueue == QueueName;
            DocumentExpirationInHours = application.DocumentExpirationInHours;

            try
            {
                RabbitManagementWebUrl = string.Format("http://{0}:{1}",
                    new Uri(application.RabbitConnectionString).Host,
                    application.RabbitManagementPort);
            }
            catch
            {
                RabbitManagementWebUrl = "#";
            }
        }

        public bool IsErrorQueue { get; private set; }
        public string ApplicationId { get; private set; }

        public string ApplicationName { get; private set; }

        public string QueueName { get; private set; }

        public int MaxMessagesPerRun { get; private set; }

        public int PollingTimeoutMilliseconds { get; private set; }

        public string RabbitConnectionString { get; private set; }

        public int HeartbeatIntervalSeconds { get; private set; }

        public int DocumentExpirationInHours { get; private set; }

        public string RabbitManagementWebUrl { get; private set; }
    }
}