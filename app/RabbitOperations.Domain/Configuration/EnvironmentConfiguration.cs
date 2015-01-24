using System.Collections.Generic;

namespace RabbitOperations.Domain.Configuration
{
    public class EnvironmentConfiguration
    {
        public string EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }
        public string AuditQueue { get; set; }
        public string ErrorQueue { get; set; }

        public string RabbitConnectionString { get; set; }

        public int MaxMessagesPerRun { get; set; }
        public int PollingTimeoutMilliseconds { get; set; }

        public int HeartbeatIntervalSeconds { get; set; }
        public IList<MessageTypeHandling> MessageHandlingInstructions { get; set; }
    }
}