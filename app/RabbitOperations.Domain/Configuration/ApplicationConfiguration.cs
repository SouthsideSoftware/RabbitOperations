using Newtonsoft.Json;

namespace RabbitOperations.Domain.Configuration
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            AuditQueue = "audit";
            ErrorQueue = "error";
            RabbitConnectionString = "amqp://localhost";
            PollingTimeoutMilliseconds = 500;
            HeartbeatIntervalSeconds = 10;
            AutoStartQueuePolling = false;
            DocumentExpirationInHours = 7*24;
            ErrorDocumentExpirationInHours = 14*24;
            RabbitManagementPort = 15672;
            Prefetch = 10;
        }


        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
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

        public bool AutoStartQueuePolling { get; set; }

        public int DocumentExpirationInHours { get; set; }

        public int ErrorDocumentExpirationInHours { get; set; }

        public int RabbitManagementPort { get; set; }
        public ushort Prefetch { get; set; }

		[JsonIgnore]
		public string ApplicationLogInfo
		{
			get
			{
				return $"{ApplicationName} ({ApplicationId})";
			}
		}
    }
}