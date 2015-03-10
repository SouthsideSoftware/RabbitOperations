using System.Collections.Generic;

namespace RabbitOperations.Domain.Configuration
{
    public class EnvironmentConfiguration
    {
        public EnvironmentConfiguration()
        {
            AuditQueue = "audit";
            ErrorQueue = "error";
            RabbitConnectionString = "amqp://localhost";
            PollingTimeoutMilliseconds = 5000;
            HeartbeatIntervalSeconds = 10;
            MessageHandlingInstructions = new List<MessageTypeHandling>();
            AutoStartQueuePolling = false;
            DocumentExpirationInHours = 30*24;
        }


        public string EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }
        public string AuditQueue { get; set; }
        public string ErrorQueue { get; set; }

        public string RabbitConnectionString { get; set; }

        /// <summary>
        /// The maximum number of messages that will be read in one run of the application.
        /// This should only be set for troubleshooting or configuration purposes.
        /// Normally, this is set to zero, which indicates no limit
        /// </summary>
        public int MaxMessagesPerRun { get; set; }
        public int PollingTimeoutMilliseconds { get; set; }

        /// <summary>
        /// The app keeps the connection alive by sending peiodic heartbeats.
        /// This should be set below 20 seconds when running behind an ELB because
        /// they use a relatively short timeout.
        /// </summary>
        public int HeartbeatIntervalSeconds { get; set; }
        public IList<MessageTypeHandling> MessageHandlingInstructions { get; set; }

        public bool AutoStartQueuePolling { get; set; }

        public int DocumentExpirationInHours { get; set; }
    }
}